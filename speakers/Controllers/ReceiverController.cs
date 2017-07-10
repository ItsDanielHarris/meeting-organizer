using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Data.SqlClient;
using speakers.Models;
using System.Diagnostics;
using System.Net;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace speakers.Controllers
{
    public class ReceiverController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public string SpeakersReorder(List<Speaker> speakers)
        {
            ViewBag.query = "";

            bool error = false;
            string error_msg = "";

            foreach (Speaker speaker in speakers)
            {
                int result = 0;
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    conn.Open();
                    try
                    {
                        SqlCommand command = conn.CreateCommand();

                        command.CommandText = "UPDATE AspNetUsers SET List_Order = " + speaker.listOrder + " WHERE Id = '" + speaker.Id + "'; UPDATE AspNetUsers SET Requested = 0 WHERE List_Order = 0;";

                        result = command.ExecuteNonQuery();
                        if (result == 0)
                        {
                            error = true;
                            error_msg = "Query failed. \nQuery: " + command.CommandText;
                        }
                    }
                    catch (SqlException e)
                    {
                        error = true;
                        error_msg = e.Message;
                    }
                }
            }
            if (!error)
            {
                error_msg = "no error";
            }
            return "Setting speakers order...\n" + error_msg;
        }

        public string set_new_speaker(string new_speaker)
        {
            Speaker current_speaker = new Speaker();

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    var query = "SELECT Id, FirstName, LastName, List_Order, TimeLeft FROM AspNetUsers WHERE LoggedIn = 1 AND Id = '" + new_speaker + "'";
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(rdr["Id"])))
                        {
                            current_speaker.Id = Convert.ToString(rdr["Id"]);
                            current_speaker.firstName = Convert.ToString(rdr["FirstName"]);
                            current_speaker.lastName = Convert.ToString(rdr["LastName"]);
                            current_speaker.listOrder = Convert.ToInt32(rdr["List_Order"]);
                            current_speaker.timeLeft = Convert.ToInt32(rdr["TimeLeft"]);
                        }
                    }
                    GlobalVars.last_speaker = GlobalVars.current_speaker;
                    GlobalVars.current_speaker = current_speaker;

                    conn.Close();

                    return "New speaker set successfully";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }

            



            


            

        }

        public ActionResult EndMeeting()
        {
            ViewBag.query = "";

            int result = 0;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "UPDATE AspNetUsers SET LoggedIn = 0, Requested = 0";

                    result = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                }
            }

            //Log off current user
            IAuthenticationManager AuthenticationManager = HttpContext.GetOwinContext().Authentication;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return View("AjaxResult", "_NoLayout");
        }

        [HttpPost]
        [OverrideAuthentication]
        public ActionResult RequestSpeaker(string email)
        {
            ViewBag.query = "";

            int result = 0;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "UPDATE AspNetUsers SET Requested = 1 WHERE Email = @speaker_email";
                    SqlParameter speaker_email = new SqlParameter("@speaker_email", email);
                    command.Parameters.Add(speaker_email);

                    result = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                }
            }
            return View("AjaxResult", "_NoLayout");
        }

        [HttpPost]
        [OverrideAuthentication]
        public ActionResult StopRequestSpeaker(string email)
        {
            ViewBag.query = "";

            int result = 0;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "UPDATE AspNetUsers SET Requested = 0 WHERE Email = @speaker_email";
                    SqlParameter speaker_email = new SqlParameter("@speaker_email", email);
                    command.Parameters.Add(speaker_email);

                    result = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                }
            }
            return View("AjaxResult", "_NoLayout");
        }

        [Authorize]
        [OverrideAuthentication]
        public ActionResult CheckRequested()
        {
            List<string> ids = new List<string>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "SELECT Id FROM AspNetUsers WHERE Requested = 1";

                    SqlDataReader reader = command.ExecuteReader();

                    while(reader.Read())
                    {
                        ids.Add((string)reader["Id"]);
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            return Json(ids, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OverrideAuthentication]
        public ActionResult CheckLoggedIn(string email)
        {
            return Json(GlobalVars.IsLoggedIn(email), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [OverrideAuthentication]
        public string get_speakers()
        {
            GlobalFuncs.get_speakers();
            return "getting speakers";
        }

        public bool same_speaker()
        {
            return GlobalVars.same_speaker();
        }

        public string get_current_speaker()
        {
            return GlobalVars.current_speaker.Id;
        }

        public string get_last_speaker()
        {
            return GlobalVars.last_speaker.Id;
        }

        [Authorize]
        [OverrideAuthentication]
        public ActionResult GetDrag()
        {
            bool drag = false;
            List<string> ids = new List<string>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "SELECT drag FROM drag";

                    drag = (bool) command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            return Json(drag, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OverrideAuthentication]
        public int SetDrag(bool drag)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "UPDATE dbo.drag SET drag = "+drag;

                    return command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    //Response.Write(e.Message);
                    return 0;
                }
            }
        }

        [HttpPost]
        [OverrideAuthentication]
        public string update_speaker_list_status(bool same_speaker)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "UPDATE status SET status = '" + !same_speaker;

                    int result = command.ExecuteNonQuery();

                    return result.ToString() + " status rows affected";
                }
                catch (SqlException e)
                {
                    return e.Message;
                }
            }
        }

        public string SetSpeakerTime()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = conn.CreateCommand();

                    command.CommandText = "UPDATE AspNetUsers SET TimeLeft = '" + GlobalVars.current_speaker.timeLeft + "' WHERE Id = '" + GlobalVars.current_speaker.Id + "'";

                    int result = command.ExecuteNonQuery();

                    //return "setting time \n" + result + " rows affected.";
                    return "setting time \nid: " + GlobalVars.current_speaker.Id + "\ntime: " + GlobalVars.current_speaker.timeLeft;
                }
                catch (SqlException e)
                {
                    return e.Message;
                }
            }      
            
        }

        public int GetTimeLeft()
        {
            return GlobalVars.current_speaker.timeLeft;
        }

        public string Tick()
        {
            if (GlobalVars.current_speaker.timeLeft > 0)
            {
                GlobalVars.current_speaker.timeLeft--;
            }
            return "Tick";
        }

        public string ResetTimeLeft()
        {
            GlobalVars.current_speaker.timeLeft = 180;
            
            return "Resetted Time Left";
        }
    }
}