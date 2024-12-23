using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace SubsApp
{
    public partial class Form1 : Form
    {
        int user = 0;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (user == 2)
            {
                Form2 newForm = new Form2();
                newForm.Show();
            }
            else
            {
                MessageBox.Show("Недостаточно прав доступа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (user != 0)
            {
                Form3 newForm1 = new Form3();
                newForm1.Show();
            }
            else
            {
                MessageBox.Show("Недостаточно прав доступа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private string GetQueryForTable(string tableName)
        {
            if (tableName == "\"Clients\"")
            {
                return @"SELECT id,
                     ""FIO"" AS ""ФИО"",
                     ""Address"" AS ""Адрес"",
                     ""PhoneNumber"" AS ""Номер телефона""
              FROM public.""Clients"";";
            }
            else if (tableName == "\"Subs\"")
            {
                return @"SELECT s.id,
             s.""clientID"" AS ""clientID"", 
             s.""journalID"" AS ""journalID"", 
             c.""FIO"" AS ""Клиент"",
             j.""Name"" AS ""Журнал"",
             CASE WHEN s.""Payment"" THEN 'Оплачено' ELSE 'Не оплачено' END AS ""Оплата"",
             s.""CardNumber"" AS ""Номер карты"",
             s.""SubTime"" AS ""Срок подписки"" 
              FROM public.""Subs"" s
              JOIN public.""Clients"" c ON s.""clientID"" = c.id
              JOIN public.""Journals"" j ON s.""journalID"" = j.id;";
            }
            else if (tableName == "\"Journals\"")
            {
                return @"SELECT j.id,
                     j.""Name"" AS ""Название"",
                     p.""Name"" AS ""Издатель"",
                     j.""LastArrival"" AS ""Последнее поступление"",
                     j.""ReleasePeriod"" AS ""Периодичность"",
                     j.""Logo"" AS ""Лого""
                 FROM public.""Journals"" j
                 JOIN public.""Publishers"" p ON j.""PublisherID"" = p.id;";
            }
            else if (tableName == "\"Publishers\"")
            {
                return @"SELECT id,
                     ""Name"" AS ""Название"",
                     ""Address"" AS ""Адрес"",
                     ""Contact"" AS ""Контакты"",
                     ""Email"" AS ""Электронная почта""
              FROM public.""Publishers"";";
            }
            else
            {
                return null;
            }
        }

        private void LoadTableData(string tableName)
        {
            string query = GetQueryForTable(tableName);

            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Запрос для данной таблицы не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; database=Subs; user Id=postgres; password=admin"))
                {
                    connection.Open();

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                        if (tableName == "\"Subs\"")
                        {
                            if (dataGridView1.Columns.Contains("clientID"))
                                dataGridView1.Columns["clientID"].Visible = false;

                            if (dataGridView1.Columns.Contains("journalID"))
                                dataGridView1.Columns["journalID"].Visible = false;

                            if (dataGridView1.Columns.Contains("id"))
                                dataGridView1.Columns["id"].Visible = false;
                        }

                        if (tableName == "\"Journals\"")
                        {
                            if (dataGridView1.Columns.Contains("id"))
                                dataGridView1.Columns["id"].Visible = false;
                        }

                        if (tableName == "\"Publishers\"")
                        {
                            if (dataGridView1.Columns.Contains("id"))
                                dataGridView1.Columns["id"].Visible = false;
                        }

                        if (tableName == "\"Clients\"")
                        {
                            if (dataGridView1.Columns.Contains("id"))
                                dataGridView1.Columns["id"].Visible = false;
                        }
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

            string currentTable = GetCurrentTableName();


            if (!int.TryParse(selectedRow.Cells["id"].Value.ToString(), out int recordId))
            {
                MessageBox.Show("Не удалось получить идентификатор записи.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            if (user == 0)
            {
                MessageBox.Show("Недостаточно прав доступа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {

                if (currentTable == "\"Publishers\"" && (user < 2))
                {
                    MessageBox.Show("Недостаточно прав доступа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (currentTable == "\"Journals\"" && (user < 2))
                    {
                        MessageBox.Show("Недостаточно прав доступа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {


                        bool hasDependencies = CheckDependencies(currentTable, recordId);

                        string warningMessage = hasDependencies
                            ? "Запись связана с другими данными. Удаление также приведет к удалению всех связанных записей. Вы уверены, что хотите продолжить?"
                            : "Вы уверены, что хотите удалить запись?";

                        DialogResult result = MessageBox.Show(warningMessage, "Подтверждение удаления", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                        if (result != DialogResult.OK)
                            return;


                        try
                        {
                            using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; database=Subs; user Id=postgres; password=admin"))
                            {
                                connection.Open();

                                string query = $"DELETE FROM public.{currentTable} WHERE id = @id";

                                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@id", recordId);
                                    command.ExecuteNonQuery();
                                }
                            }

                            MessageBox.Show("Запись успешно удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadTableData(currentTable);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка при удалении записи: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
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
            if (user != 0)
            {
                Form4 newForm2 = new Form4();
                newForm2.Show();
            }
            else
            {
                MessageBox.Show("Недостаточно прав доступа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (user != 0)
            {
                Form5 newForm3 = new Form5();
                newForm3.Show();
            }
            else
            {
                MessageBox.Show("Недостаточно прав доступа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (user == 2)
            {
                Form6 newForm4 = new Form6();
                newForm4.Show();
            }
            else
            {
                MessageBox.Show("Недостаточно прав доступа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "worker" && textBox2.Text == "worker")
            {
                user = 1;
                textBox1.Clear();
                textBox2.Clear();
            }

            if (textBox1.Text == "admin" && textBox2.Text == "admin")
            {
                user = 2;
                textBox1.Clear();
                textBox2.Clear();
            }

            if (user == 1)
            {
                label5.Visible = true;
                label6.Visible = false;
                button8.Visible = true;
            }
            if (user == 2)
            {
                label6.Visible = true;
                label5.Visible = false;
                button8.Visible = true;
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            user = 0;
            label5.Visible = false;
            label6.Visible = false;
            button8.Visible = false;
            textBox1.Clear();
            textBox2.Clear();

        }

        private bool CheckDependencies(string tableName, int recordId)
        {
            string query = null;

            if (tableName == "\"Clients\"")
            {
                query = "SELECT COUNT(*) FROM public.\"Subs\" WHERE \"clientID\" = @id";
            }
            else if (tableName == "\"Journals\"")
            {
                query = "SELECT COUNT(*) FROM public.\"Subs\" WHERE \"journalID\" = @id";
            }
            else if (tableName == "\"Publishers\"")
            {
                query = "SELECT COUNT(*) FROM public.\"Journals\" WHERE \"PublisherID\" = @id";
            }

            if (query == null)
                return false;

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; database=Subs; user Id=postgres; password=admin"))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", recordId);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке зависимых записей: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string searchText = textBox3.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Пожалуйста, введите текст для поиска.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataGridView1.DataSource is DataTable originalTable)
            {
                DataTable filteredTable = originalTable.Clone(); 

                foreach (DataRow row in originalTable.Rows)
                {
                    if (row.ItemArray.Any(cell => cell.ToString().ToLower().Contains(searchText)))
                    {
                        filteredTable.ImportRow(row); 
                    }
                }

                dataGridView1.DataSource = filteredTable;
            }
        }
    }
}
