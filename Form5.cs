using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubsApp
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PreviewReport(string query, string reportTitle)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; database=Subs; user Id=postgres; password=admin"))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {

                        StringBuilder reportBuilder = new StringBuilder();
                        reportBuilder.AppendLine(reportTitle);
                        reportBuilder.AppendLine(new string('-', reportTitle.Length));


                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            reportBuilder.Append(reader.GetName(i).PadRight(60));
                        }
                        reportBuilder.AppendLine();
                        reportBuilder.AppendLine(new string('-', 60 * reader.FieldCount));


                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                reportBuilder.Append(reader[i].ToString().PadRight(60));
                            }
                            reportBuilder.AppendLine();
                        }


                        textBox1.Text = reportBuilder.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка формирования отчета: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
                try
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                        saveFileDialog.Title = "Сохранить отчет";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllText(saveFileDialog.FileName, textBox1.Text);
                            MessageBox.Show("Отчет успешно сохранен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            string query = @"SELECT 
                                p.""Name"" AS ""Издательство"",
                                j.""Name"" AS ""Журнал"",
                                COUNT(s.""journalID"") AS ""Количество подписок""
                            FROM 
                                public.""Subs"" s
                            JOIN 
                                public.""Journals"" j ON s.""journalID"" = j.id
                            JOIN 
                                public.""Publishers"" p ON j.""PublisherID"" = p.id
                            GROUP BY 
                                p.""Name"", j.""Name""
                            ORDER BY 
                                COUNT(s.""journalID"") DESC;";
            PreviewReport(query, "Заказ журналов у издательства");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            string query = @"
                            SELECT 
                            c.""FIO"" AS ""Клиент"", 
                            j.""Name"" AS ""Журнал""
                        FROM 
                            public.""Subs"" s
                        JOIN 
                            public.""Clients"" c ON s.""clientID"" = c.id
                        JOIN 
                            public.""Journals"" j ON s.""journalID"" = j.id
                        ORDER BY 
                            c.id;";
            PreviewReport(query, "Реестр подписчиков");


        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            string query = @"SELECT j.""Name"" AS ""Журнал"",  
                                COUNT(s.""journalID"") AS ""Количество подписок"" 
                                FROM public.""Subs"" s 
                                JOIN public.""Journals"" j ON   s.""journalID"" = j.id GROUP BY j.""Name"" 
                                ORDER BY COUNT(s.""journalID"") DESC";
            PreviewReport(query, "Рейтинг журналов");

            //CALL public.generate_publisher_rating();
        }
    }
}
