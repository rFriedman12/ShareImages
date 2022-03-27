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
            cmd.CommandText = @"INSERT INTO Images (FileName, FilePath, Password) VALUES (@fileName, @filePath, @password)
                                SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@fileName", image.FileName);
            cmd.Parameters.AddWithValue("@filePath", image.FilePath);
            cmd.Parameters.AddWithValue("@password", image.Password);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }
    }
}
