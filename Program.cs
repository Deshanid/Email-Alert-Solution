
using ExpiryEmailAlertSolution.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExpirySolution
{
    class Program
    {
        public static void Main()
        {

            EmailTemplateInfo();
        }



        public static void EmailTemplateInfo()
        {
            bool status = false;
            
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    Console.WriteLine("Successfully connected to the SQL server.");

                    var emailTemplateInfos = GetEmailTemplateInfo(sqlConnection);

                    foreach (var library in emailTemplateInfos)
                    {
                        string listTo = library.To;
                        string listCC = library.Cc;
                        string from = library.From;
                        string subject = library.Subject;
                        string fromPWD = library.FromPWD;
                        string emailBody = library.BodyHeader;
                        string[] indexes = library.Index.Split(',');
                        List<string> days = library.AlertinDays.Split(',').ToList();

                        foreach (var day in days)
                        {
                            int d = Int16.Parse(day);
                            subject = string.Format(library.Subject, day);

                            string sql1 = "Select " + library.Index + " from " + library.SqlViewName + " Where [Expiry Date] = '" + (DateTime.Today.AddDays(d)).ToString() + "'";
                            

                            library.BodyFooter = string.Format(library.BodyFooter);

                            emailBody = string.Format(library.BodyHeader, d);


                            SqlCommand cmd1 = new SqlCommand(sql1, sqlConnection);
                            // List<LibraryIndexes> libraryIndexes1 = new List<LibraryIndexes>();

                            try
                            {
                                Console.WriteLine("Documents related to each library were selected.");
                                SqlDataReader reader1;
                                reader1 = cmd1.ExecuteReader();
                                DataTable dt = new DataTable();
                                dt.Load(reader1);
                                Console.WriteLine("Loading executing data");
                                int numRows = dt.Rows.Count;
                                int ix = 0;

                                foreach (DataRow ind in dt.Rows)
                                {
                                    ix++;
                                    emailBody += "<table border=0>";
                                    List<string> values = new List<string>();
                                    int iy = 0;
                                    foreach (string index in indexes)
                                    {

                                        String Info = index.Replace("[", "").Replace("]", "");
                                        emailBody += "<tr><td width = '200px'>" + Info + "</td>" +
                                         "<td width='100px'>" + Convert.ToString(ind.ItemArray[iy]) + "</td>" +
                                         "</tr>";
                                        iy++;

                                       
                                    }
                                    Console.WriteLine("The email body is set as the document " + ix);




                                    emailBody += "</table ></br></br>";


                                    if (numRows == ix)
                                    {
                                        emailBody += library.BodyFooter;

                                        status = SendEmail(listTo
                                         , listCC
                                         , subject
                                         , emailBody
                                         , from
                                         , fromPWD);
                                        string success = string.Format(@"Successfully send Email. Library Name : {0} To : {1} Cc : {2} Subject : {3} "
                                            ,library.LibraryName,library.To,library.Cc,subject
                                            );
                                        bool text =true;
                                        Print("EmailTemplateInfo", success,text);
        
                                                Console.WriteLine("Email send to the relevent parties of " + library.LibraryName);
                                        Console.WriteLine(" ");
                                    }


                                }

                                
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error : " + e);
                                Console.ReadLine();

                                string success = string.Format(@"Email Send Failed. Library Name : {0}  To : {1}  Cc : {2}  Error : {3}"
                                            , library.LibraryName, library.To, library.Cc, e
                                            );
                                bool text = false;
                                Print("EmailTemplateInfo", success,text);
                            }


                        }

                    }


                }
                Console.ReadLine();
                return;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex);
                bool text = false;
                Print("EmailTemplateInfo", "Error : " +ex, text);
                return;

            }
        }

       

        public static List<EmailTemplateInfo> GetEmailTemplateInfo(SqlConnection sqlConnection)
        {
            try
            {
                List<EmailTemplateInfo> emailtemplateInfos = new List<EmailTemplateInfo>();
                string sql = "Select ID,[LibraryName],[To],Cc,[From],FromPWD,BodyHeader,BodyFooter,Subject,[Index],[SqlViewName],[AlertInDays] from EmailTemplate";

                Console.WriteLine("Fields relevent to each library were selected.");

                SqlCommand cmd = new SqlCommand(sql, sqlConnection);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    EmailTemplateInfo emailTemplateInfo = new EmailTemplateInfo()
                    {
                        ID = (int)reader["ID"],
                        LibraryName = (string)reader["LibraryName"],
                        To = (string)reader["To"],
                        Cc = (string)reader["Cc"],
                        From = (string)reader["From"],
                        FromPWD = (string)reader["FromPWD"],
                        BodyHeader = (string)reader["BodyHeader"],
                        BodyFooter = (string)reader["BodyFooter"],
                        Subject = (string)reader["Subject"],
                        Index = (string)reader["Index"],
                        SqlViewName = (string)reader["SqlViewName"],
                        AlertinDays = (string)reader["AlertInDays"]


                    };
                    emailtemplateInfos.Add(emailTemplateInfo);
                    Console.WriteLine("Adding relevent data of library to the entity.");
                }
                reader.Close();
                Console.WriteLine("Executed Reader was closed.");
                return emailtemplateInfos;
            }
            catch (Exception ex)
            {
                bool text = false;
                Print("GetEmailTemplateInfo", "Error : " + ex,text);

                throw ex;
            }
        }


        public static bool SendEmail(string listTo
         , string listCC
         , string subject
         , string msg
         , string from
            , string fromPWD)
        {
            bool status = true;
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(from);
                mail.To.Add(listTo);
                mail.CC.Add(listCC);

                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Credentials = new NetworkCredential(from, fromPWD);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                }
            }
            catch (Exception ex)
            {
                string error = string.Format(@"Class :-{0},Method :-{1}, Msg :-{2}"
                     , "Program"
                     , "SendEmail"
                     , ex.Message);
                Console.WriteLine("Email send failed. Error : " + error);

                bool text = false;
                Print("SendEmail", "Error : " + ex, text);
            }
            return status;
        }

        public static void Print(string method, string msg, bool text) {
            
            var _class = typeof(Program);
            var _namespace = _class.Namespace;
            



            if (text == true) {
                
                Logger.PrintExecutionLog(_namespace, "Program", method, msg);
                return ;

            }

            else
            {
                Logger.PrintError(_namespace, "Program" , method, msg);
                return ;
            }
            }
    }
}
