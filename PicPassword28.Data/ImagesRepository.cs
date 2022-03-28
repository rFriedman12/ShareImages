using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPassword28.Data
{
    public class ImagesRepository
    {
        private string _connString;

        public ImagesRepository(string connString)
        {
            _connString = connString;
        }

        public int AddImage(Image image)
        {
            using SqlConnection conn = new(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Images (FileName, FilePath, Password, Views) VALUES (@fileName, @filePath, @password, 0)
                                SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@fileName", image.FileName);
            cmd.Parameters.AddWithValue("@filePath", image.FilePath);
            cmd.Parameters.AddWithValue("@password", image.Password);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public Image GetImage(int id)
        {
            using SqlConnection conn = new(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var reader = cmd.ExecuteReader();
            var image = new Image
            {
                Id = id
            };
            while (reader.Read())
            {
                image.FileName = (string)reader["FileName"];                
                image.Password = (string)reader["Password"];
                image.Views = (int)reader["Views"];
            }
            return image;
        }

        public void IncreaseViews(int id)
        {
            using SqlConnection conn = new(_connString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Images SET Views = Views + 1 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
