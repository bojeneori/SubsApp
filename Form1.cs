using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace SubsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 newForm = new Form2();
            newForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 newForm1 = new Form3();
            newForm1.Show();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем имя таблицы через метод GetCurrentTableName
            string tableName = GetCurrentTableName();

            if (tableName != null)
            {
                LoadTableData(tableName);
            }
            else
            {
                MessageBox.Show("Выберите допустимый элемент.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadTableData(string tableName)
        {
            string query = $"SELECT * FROM public.{tableName}";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; database=Subs; user Id =postgres; password =admin "))
                {
                    connection.Open();

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];


            if (!int.TryParse(selectedRow.Cells["id"].Value.ToString(), out int recordId))
            {
                MessageBox.Show("Не удалось получить идентификатор записи.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить запись?",
                                                  "Подтверждение удаления",
                                                  MessageBoxButtons.OKCancel,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {

                try
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; database=Subs; user Id =postgres; password =admin "))
                    {
                        connection.Open();
                        string query = $"DELETE FROM public.{GetCurrentTableName()} WHERE id = @id";

                        using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", recordId);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Запись успешно удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    LoadTableData(GetCurrentTableName());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении записи: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GetCurrentTableName()
        {
            string selectedItem = listBox1.SelectedItem.ToString();

            if (selectedItem == "Клиенты") return "\"Clients\"";
            if (selectedItem == "Подписки") return "\"Subs\"";
            if (selectedItem == "Журналы") return "\"Journals\"";
            if (selectedItem == "Издатели") return "\"Publishers\"";

            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Form4 newForm2 = new Form4();
            newForm2.Show();
        }
    }
    }
