using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace speakers.Models
{
    public static class GlobalVars
    {
        public static Speaker user {
            get {
                string query = "SELECT Id, FirstName, LastName, List_Order, TimeLeft FROM AspNetUsers WHERE Id = '" + HttpContext.Current.User.Identity.GetUserId() + "'";
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    Speaker user = new Speaker();
                    while (rdr.Read())
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(rdr["Id"])))
                        {
                            Speaker speaker = new Speaker();
                            user.Id = Convert.ToString(rdr["Id"]);
                            user.firstName = Convert.ToString(rdr["FirstName"]);
                            user.lastName = Convert.ToString(rdr["LastName"]);
                            user.listOrder = Convert.ToInt32(rdr["List_Order"]);
                            user.timeLeft = Convert.ToInt32(rdr["TimeLeft"]);
                        }
                    }
                    return user;
                }
            }
        }

        public static Speaker current_speaker { get; set; }

        public static List<Speaker> current_speakers { get; set; }

        public static Speaker last_speaker { get; set; }

        public static bool same_speaker() {
            return current_speaker.Id == last_speaker.Id;
        }

        public static string Error { get; set; }

        public static bool IsLoggedIn(string email)
        {
            bool isloggedin = false;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "SELECT LoggedIn FROM AspNetUsers WHERE Email = @speaker_email";
                    SqlParameter speaker_email = new SqlParameter("@speaker_email", email);
                    command.Parameters.Add(speaker_email);

                    object result = command.ExecuteScalar();
                    isloggedin = Convert.ToInt16(result.ToString()) == 1;
                }
                catch (Exception e)
                {

                }
            }
            return isloggedin;
        }

        public static bool SetLoggedIn(bool loggedin, string email)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = conn.CreateCommand();
                    command.CommandText = "UPDATE AspNetUsers SET TimeLeft = 180, LoggedIn = " + (loggedin ? "1" : "0") + ", List_Order=(SELECT MAX(List_Order)+1 FROM AspNetUsers), Requested = 0 WHERE dbo.AspNetUsers.UserName = @email";
                    SqlParameter par_email = new SqlParameter("@email", email);
                    command.Parameters.Add(par_email);

                    command.ExecuteNonQuery();


                    SqlCommand command3 = conn.CreateCommand();
                    command3.CommandText = "SELECT * FROM AspNetUsers WHERE UserName = @email";
                    SqlParameter para_email = new SqlParameter("@email", email);
                    command3.Parameters.Add(para_email);

                    System.Data.IDataReader reader = command3.ExecuteReader();
                    while(reader.Read())
                    {
                        user.firstName = (string) reader.GetString(reader.GetOrdinal("FirstName"));
                        user.lastName = (string) reader.GetString(reader.GetOrdinal("LastName"));
                        user.email = (string) reader.GetString(reader.GetOrdinal("Email"));
                        user.Id = (string) reader.GetString(reader.GetOrdinal("Id"));
                    }


                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }


    }
}