using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubsApp
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; database=Subs; user Id=postgres; password=admin"))
                {
                    connection.Open();


                    using (NpgsqlCommand clientCommand = new NpgsqlCommand("SELECT id, \"FIO\" FROM public.\"Clients\"", connection))
                    using (NpgsqlDataReader clientReader = clientCommand.ExecuteReader())
                    {
                        DataTable clientsTable = new DataTable();
                        clientsTable.Load(clientReader);


                        listBox1.DisplayMember = "FIO";  
                        listBox1.ValueMember = "id";    
                        listBox1.DataSource = clientsTable;
                    }


                    using (NpgsqlCommand journalCommand = new NpgsqlCommand("SELECT id, \"Name\" FROM public.\"Journals\"", connection))
                    using (NpgsqlDataReader journalReader = journalCommand.ExecuteReader())
                    {
                        DataTable journalsTable = new DataTable();
                        journalsTable.Load(journalReader);


                        listBox2.DisplayMember = "Name";  
                        listBox2.ValueMember = "id";     
                        listBox2.DataSource = journalsTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                if (listBox1.SelectedValue == null)
                {
                    MessageBox.Show("Выберите клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (listBox2.SelectedValue == null)
                {
                    MessageBox.Show("Выберите журнал.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!checkBox1.Checked)
                {
                    MessageBox.Show("Внесите полную предоплату", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (maskedTextBox1.Text == null)
                {
                    MessageBox.Show("Введите номер банковской карты", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                string subTime = listBox3.Text.Trim();
                if (string.IsNullOrEmpty(subTime))
                {
                    MessageBox.Show("Введите срок подписки.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                int clientID = Convert.ToInt32(listBox1.SelectedValue);
                int journalID = Convert.ToInt32(listBox2.SelectedValue);


                bool isPaid = checkBox1.Checked; 


                using (NpgsqlConnection connection = new NpgsqlConnection("server=localhost; database=Subs; user Id=postgres; password=admin"))
                {
                    connection.Open();

                    string query = "INSERT INTO public.\"Subs\" (\"clientID\", \"journalID\", \"Payment\", \"CardNumber\", \"SubTime\") " +
                                   "VALUES (@clientID, @journalID, @Payment, @CardNumber, @SubTime)";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@clientID", clientID);
                        command.Parameters.AddWithValue("@journalID", journalID);
                        command.Parameters.AddWithValue("@Payment", isPaid);
                        command.Parameters.AddWithValue("@CardNumber", maskedTextBox1.Text);
                        command.Parameters.AddWithValue("@SubTime", subTime);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Подписка успешно добавлена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения подписки: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
