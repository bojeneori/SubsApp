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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NpgsqlConnection conn = new NpgsqlConnection("server=localhost; database=Subs; user Id =postgres; password =admin ");

            // Создадим новый набор данных
            DataSet datasetmain = new DataSet();
            // Открываем подключение
            conn.Open();
            // Очищаем набор данных
            datasetmain.Clear();
        }
    }
}
