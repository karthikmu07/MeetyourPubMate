using CoupleEntry.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CoupleEntry
{
    public class DALayer
    {
        // private static string _connectionString = "Data Source=bgrsql01;;Initial Catalog=LocalTest;Integrated Security=False;user id=sa;password=Squeeze66";
        private static string _connectionString = "Data Source=184.168.194.53;Initial Catalog=LocalTest;Integrated Security=False;user id=parassaxena3;password=P@ssw0rd1=2-";

        public static object DataAccess { get; private set; }

        public static bool IsEmailPresentInDB(string emailId)
        {
            string procName = "IsEmailPresentInDB";
            SqlParameter returnParameter = new SqlParameter() { ParameterName = "IsExists", SqlDbType = SqlDbType.Bit, Direction = ParameterDirection.Output };
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "EmailId", SqlDbType = SqlDbType.VarChar, Value = emailId });
            command.Parameters.Add(returnParameter);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            return Convert.ToBoolean(returnParameter.Value);
        }

        public static void UpsertTokenValue(string tokenId, string emailId)
        {
            string procName = "UpsertTokenValue";
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "CookieId", SqlDbType = SqlDbType.NVarChar, Value = tokenId });
            command.Parameters.Add(new SqlParameter() { ParameterName = "EmailId", SqlDbType = SqlDbType.NVarChar, Value = emailId });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        public static void UpsertUserPosition(string emailId, string latitude, string longitude)
        {
            string procName = "UpsertUserPosition";
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "EmailId", SqlDbType = SqlDbType.NVarChar, Value = emailId });
            command.Parameters.Add(new SqlParameter() { ParameterName = "Latitude", SqlDbType = SqlDbType.NVarChar, Value = latitude });
            command.Parameters.Add(new SqlParameter() { ParameterName = "Longitude", SqlDbType = SqlDbType.NVarChar, Value = longitude });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }

        public static User AddNewUser(LoginModel model)
        {
            User currentUser = new User();
            string procName = "AddNewUser";
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "UserName", SqlDbType = SqlDbType.NVarChar, Value = model.Username });
            command.Parameters.Add(new SqlParameter() { ParameterName = "Age", SqlDbType = SqlDbType.Int, Value = model.Age });
            command.Parameters.Add(new SqlParameter() { ParameterName = "Gender", SqlDbType = SqlDbType.NVarChar, Value = model.Gender });
            command.Parameters.Add(new SqlParameter() { ParameterName = "ImageUrl", SqlDbType = SqlDbType.NVarChar, Value = model.ImageUrl });
            command.Parameters.Add(new SqlParameter() { ParameterName = "Name", SqlDbType = SqlDbType.NVarChar, Value = model.Name });
            command.Parameters.Add(new SqlParameter() { ParameterName = "EmailId", SqlDbType = SqlDbType.NVarChar, Value = model.Email });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();

                using (IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            currentUser.Name = GetStringFromReader("Name", reader);
                            currentUser.EmailId = GetStringFromReader("EmailId", reader);
                            currentUser.ImageUrl = GetStringFromReader("ImageUrl", reader);
                            currentUser.Age = Convert.ToInt32(GetStringFromReader("Age", reader));
                            currentUser.Gender = GetStringFromReader("Gender", reader);
                            currentUser.Latitude = GetStringFromReader("Latitude", reader);
                            currentUser.Longitude = GetStringFromReader("Longitude", reader);
                            currentUser.LastSeen = Convert.ToDateTime(GetStringFromReader("LastSeen", reader));
                            currentUser.Username = GetStringFromReader("UserName", reader);

                        }
                    }
                }

                connection.Close();
            }
            return currentUser;
        }

        public static List<User> GetAllUsers(string emailId)
        {
            List<User> allUsers = new List<User>();

            string procName = "GetAllOtherUsers";
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "EmailId", SqlDbType = SqlDbType.NVarChar, Value = emailId });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();


                using (IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            int userId = Convert.ToInt32(GetStringFromReader("UserId", reader));
                            string name = GetStringFromReader("Name", reader);
                            string emailid = GetStringFromReader("EmailId", reader);
                            string imageurl = GetStringFromReader("ImageUrl", reader);
                            int age = Convert.ToInt32(GetStringFromReader("Age", reader));
                            string gender = GetStringFromReader("Gender", reader);
                            string latitude = GetStringFromReader("Latitude", reader);
                            string longitude = GetStringFromReader("Longitude", reader);
                            string username = GetStringFromReader("UserName", reader);
                            DateTime lastseen = Convert.ToDateTime(GetStringFromReader("LastSeen", reader));

                            allUsers.Add(new User() { UserId = userId, Name = name, EmailId = emailid, ImageUrl = imageurl, Age = age, Gender = gender, Latitude = latitude, Longitude = longitude, LastSeen = lastseen, Username = username });
                        }
                    }
                }

                connection.Close();
            }


            return allUsers;
        }

        public static List<User> GetMatchedUsers(string matches)
        {
            List<User> allUsers = new List<User>();

            string procName = "GetMatchedUsers";
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "@MatchedUsers", SqlDbType = SqlDbType.NVarChar, Value = matches });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();


                using (IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            int userId = Convert.ToInt32(GetStringFromReader("UserId", reader));
                            string name = GetStringFromReader("Name", reader);
                            string emailid = GetStringFromReader("EmailId", reader);
                            string imageurl = GetStringFromReader("ImageUrl", reader);
                            int age = Convert.ToInt32(GetStringFromReader("Age", reader));
                            string gender = GetStringFromReader("Gender", reader);
                            string latitude = GetStringFromReader("Latitude", reader);
                            string longitude = GetStringFromReader("Longitude", reader);
                            string username = GetStringFromReader("UserName", reader);
                            DateTime lastseen = Convert.ToDateTime(GetStringFromReader("LastSeen", reader));

                            allUsers.Add(new User() { UserId = userId, Name = name, EmailId = emailid, ImageUrl = imageurl, Age = age, Gender = gender, Latitude = latitude, Longitude = longitude, LastSeen = lastseen, Username = username });
                        }
                    }
                }

                connection.Close();
            }


            return allUsers;
        }

        public static User GetUserInfo(string emailId)
        {
            User currentUser = new User();

            string procName = "GetUserInfo";
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "EmailId", SqlDbType = SqlDbType.NVarChar, Value = emailId });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();

                using (IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            currentUser.UserId = Convert.ToInt32(GetStringFromReader("UserId", reader));
                            currentUser.Name = GetStringFromReader("Name", reader);
                            currentUser.EmailId = GetStringFromReader("EmailId", reader);
                            currentUser.ImageUrl = GetStringFromReader("ImageUrl", reader);
                            currentUser.Age = Convert.ToInt32(GetStringFromReader("Age", reader));
                            currentUser.Gender = GetStringFromReader("Gender", reader);
                            currentUser.Latitude = GetStringFromReader("Latitude", reader);
                            currentUser.Longitude = GetStringFromReader("Longitude", reader);
                            currentUser.LastSeen = Convert.ToDateTime(GetStringFromReader("LastSeen", reader));
                            currentUser.Username = GetStringFromReader("UserName", reader);
                            currentUser.Likes = GetStringFromReader("Likes", reader)?.Split(',').ToList();
                            currentUser.Matches = GetStringFromReader("Matches", reader)?.Split(',').ToList();
                            if (currentUser.Likes == null)
                                currentUser.Likes = new List<string>();
                            if (currentUser.Matches == null)
                                currentUser.Matches = new List<string>();
                            currentUser.Likes.Remove("");
                            currentUser.Matches.Remove("");

                        }
                    }
                }

                connection.Close();
            }


            return currentUser;
        }
        public static User UpdateUserInfo(User model)
        {
            User currentUser = new User();

            string procName = "UpdateUserInfo";
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "EmailId", SqlDbType = SqlDbType.NVarChar, Value = model.EmailId });
            command.Parameters.Add(new SqlParameter() { ParameterName = "Name", SqlDbType = SqlDbType.NVarChar, Value = model.Name });
            command.Parameters.Add(new SqlParameter() { ParameterName = "Age", SqlDbType = SqlDbType.Int, Value = model.Age });
            command.Parameters.Add(new SqlParameter() { ParameterName = "Gender", SqlDbType = SqlDbType.NVarChar, Value = model.Gender });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();

                using (IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            currentUser.Name = GetStringFromReader("Name", reader);
                            currentUser.EmailId = GetStringFromReader("EmailId", reader);
                            currentUser.ImageUrl = GetStringFromReader("ImageUrl", reader);
                            currentUser.Age = Convert.ToInt32(GetStringFromReader("Age", reader));
                            currentUser.Gender = GetStringFromReader("Gender", reader);
                            currentUser.Latitude = GetStringFromReader("Latitude", reader);
                            currentUser.Longitude = GetStringFromReader("Longitude", reader);
                            currentUser.LastSeen = Convert.ToDateTime(GetStringFromReader("LastSeen", reader));
                            currentUser.Username = GetStringFromReader("UserName", reader);

                        }
                    }
                }

                connection.Close();
            }


            return currentUser;
        }

        public static bool AddOrRemoveLike(int userId, int targetId, bool liked)
        {
            string procName = "AddOrRemoveLike";
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter returnParameter = new SqlParameter() { ParameterName = "match", SqlDbType = SqlDbType.Bit, Direction = ParameterDirection.Output };
           // SqlParameter returnParameter2 = new SqlParameter() { ParameterName = "error", SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Output };
            command.Parameters.Add(new SqlParameter() { ParameterName = "userId", SqlDbType = SqlDbType.Int, Value = userId });
            command.Parameters.Add(new SqlParameter() { ParameterName = "targetId", SqlDbType = SqlDbType.Int, Value = targetId });
            command.Parameters.Add(new SqlParameter() { ParameterName = "liked", SqlDbType = SqlDbType.Bit, Value = liked });
            command.Parameters.Add(returnParameter);
          //  command.Parameters.Add(returnParameter2);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            return Convert.ToBoolean(returnParameter.Value);
            
        }

        public static void UpdateImageUrl(string emailId, string imageUrl)
        {
            string procName = "UpdateImageUrl";
            IDbCommand command = new SqlCommand(procName);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter() { ParameterName = "ImageUrl", SqlDbType = SqlDbType.NVarChar, Value = imageUrl });
            command.Parameters.Add(new SqlParameter() { ParameterName = "EmailId", SqlDbType = SqlDbType.NVarChar, Value = emailId });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        public static string GetStringFromReader(string column, IDataReader reader)
        {
            string value = null;

            int index = reader.GetOrdinal(column);

            if (!reader.IsDBNull(index))
            {
                value = reader.GetValue(index)?.ToString()?.Trim();
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
            }

            return value;
        }
    }
}