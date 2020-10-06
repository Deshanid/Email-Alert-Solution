using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpiryEmailAlertSolution.Entity
{
    class EmailTemplateInfo
    {
        public int ID { get; set; }
        public string LibraryName { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string From { get; set; }
        public string FromPWD { get; set; }
        public string BodyHeader { get; set; }
        public string BodyFooter { get; set; }
        public string Subject { get; set; }
        public string Index { get; set; }
        public string SqlViewName { get; set; }
        public string AlertinDays { get; set; }


        public EmailTemplateInfo(int ID
            , string LibraryName
            , string To
            , string Cc
            , string From
            , string FromPWD
            , string BodyHeader
            , string BodyFooter
            , string Subject
            , string Index
            , string SqlViewName
            , string AlertInDays)
        {
            this.ID = ID;
            this.LibraryName = LibraryName;
            this.To = To;
            this.Cc = Cc;
            this.From = From;
            this.FromPWD = FromPWD;
            this.BodyHeader = BodyHeader;
            this.BodyFooter = BodyFooter;
            this.Subject = Subject;
            this.Index = Index;
            this.SqlViewName = SqlViewName;
            this.AlertinDays = AlertInDays;

        }

        public EmailTemplateInfo()
        {
        }
    }
}
