using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SubsApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

        }

        public class Publisher
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Не удалось загрузить изображение: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textBox1.Text) ||
                string.IsNullOrEmpty(listBox2.Text) ||
                pictureBox1.Image == null ||
                listBox1.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            string name = textBox1.Text;
            DateTime lastArrival = dateTimePicker1.Value;
            string releasePeriod = listBox2.Text;
            byte[] logo = ImageToByteArray(pictureBox1.Image); 
            int publisherID = (int)listBox1.SelectedValue; 

            string connectionString = "server=localhost; database=Subs; user Id =postgres; password =admin ";

            string query = "INSERT INTO public.\"Journals\" (\"Name\", \"Logo\", \"LastArrival\", \"ReleasePeriod\", \"PublisherID\") " +
                           "VALUES (@Name, @Logo, @LastArrival, @ReleasePeriod, @PublisherID)";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Logo", logo);
                        command.Parameters.AddWithValue("@LastArrival", lastArrival);
                        command.Parameters.AddWithValue("@ReleasePeriod", releasePeriod);
                        command.Parameters.AddWithValue("@PublisherID", publisherID);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string connectionString = "server=localhost; database=Subs; user Id =postgres; password =admin ";
            string query = "SELECT id, \"Name\" FROM public.\"Publishers\"";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            listBox1.DataSource = null;
                            listBox1.Items.Clear();


                            var publishers = new List<Publisher>();

                            while (reader.Read())
                            {
                                publishers.Add(new Publisher
                                {
                                    ID = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                            }

                            listBox1.DataSource = publishers;
                            listBox1.DisplayMember = "Name";
                            listBox1.ValueMember = "ID";
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки издателей: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
