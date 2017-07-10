using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace speakers.Models
{
    public static class GlobalFuncs
    {
        public static void get_speakers()
        {
            string query = "";
            try
            {
                query = "SELECT Id, FirstName, LastName, List_Order, TimeLeft FROM AspNetUsers WHERE LoggedIn = 1 ORDER BY List_Order ASC";
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    List<Speaker> speakers = new List<Speaker>();
                    while (rdr.Read())
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(rdr["Id"])))
                        {
                            Speaker speaker = new Speaker();
                            speaker.Id = Convert.ToString(rdr["Id"]);
                            speaker.firstName = Convert.ToString(rdr["FirstName"]);
                            speaker.lastName = Convert.ToString(rdr["LastName"]);
                            speaker.listOrder = Convert.ToInt32(rdr["List_Order"]);
                            speaker.timeLeft = Convert.ToInt32(rdr["TimeLeft"]);

                            speakers.Add(speaker);
                        }
                    }

                    GlobalVars.last_speaker = GlobalVars.current_speaker;
                    GlobalVars.current_speaker = speakers[0];
                    GlobalVars.current_speakers = speakers;        
                }

            }
            catch (Exception e)
            {
                var errors = new string[2] { e.Message, query };
                GlobalVars.Error = e.Message;
            }
        }

        public static bool is_current_requested()
        {
            bool isrequested = false;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "SELECT Requested FROM AspNetUsers WHERE email = @speaker_email";
                    SqlParameter speaker_email = new SqlParameter("@speaker_email", HttpContext.Current.User.Identity.Name);
                    command.Parameters.Add(speaker_email);

                    object result = command.ExecuteScalar();
                    isrequested = Convert.ToInt16(result.ToString()) == 1;
                }
                catch (Exception e)
                {

                }
            }
            return isrequested;
        }


    }
}