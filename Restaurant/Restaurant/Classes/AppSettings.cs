using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Classes
{
    public class AppSettings
    {
        public static ResourceManager resourcemanager;
        public static ResourceManager resourcemanagerreport;

        public static string lang = "en";
        public static string Reportlang = "en";

        public static string defaultPath = "";

        internal static int? itemCost;
        public static CountryCode Region;
        public static string Currency = "KD";
        public static int CurrencyId;


        public static string companyName;
        public static string Email;
        public static string Fax;
        public static string Mobile;
        public static string Address;
        public static string Phone;

        public static string sale_copy_count;
        public static string pur_copy_count;
        public static string print_on_save_sale;
        public static string print_on_save_pur;
        public static string email_on_save_sale;
        public static string email_on_save_pur;
        public static string rep_printer_name;
        public static string sale_printer_name;
        public static string salePaperSize;
        public static string rep_print_count;
        public static string docPapersize;
        public static string Allow_print_inv_count;
        public static string show_header;

        internal static int? isInvTax;
        internal static decimal? tax;
        //tax
        internal static bool? invoiceTax_bool = true;
        internal static decimal? invoiceTax_decimal = 5;
        internal static bool? itemsTax_bool = true;
        internal static decimal? itemsTax_decimal;

        internal static string dateFormat;
        internal static string accuracy;
        internal static string timeFormat;
        // hour
        static public double time_staying = 3;
        // hour
        static public double maximumTimeToKeepReservation = 3;
        // minutes
        static public int warningTimeForLateReservation = 30;

    }
}
