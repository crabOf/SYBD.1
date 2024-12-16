using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Добавьте эту директиву

namespace БдUpdate
{
    public partial class Form1 : Form
    {
        DataSet НаборДанных;
        MySqlDataAdapter Адаптер; // Измените на MySqlDataAdapter
        MySqlConnection Подключение; // Измените на MySqlConnection
        MySqlCommand Команда; // Измените на MySqlCommand

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            НаборДанных = new DataSet();
            Подключение = new MySqlConnection(
                "Server=localhost;Database=my_database;UserID=root;"); // Измените строку подключения
            Команда = new MySqlCommand();
            button1.Text = "Читать из БД"; button1.TabIndex = 0;
            button2.Text = "Сохранить в БД";
            button3.Text = "Удаление строки в БД";
            button4.Text = "Сформировать отчет"; // Добавьте эту строку
            button4.TabIndex = 3; // Установите индекс табуляции
            button4.Click += new EventHandler(button4_Click); // Подписка на событие
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Создание нового окна отчета
            ReportForm reportForm = new ReportForm(НаборДанных);
            reportForm.ShowDialog(); // Открытие формы как модального окна
        }

        private void button1_Click(object sender, EventArgs e) // Читать из БД
        {
            // Открытие подключения к базе данных
            if (Подключение.State == ConnectionState.Closed) Подключение.Open();

            // Очистка существующих данных в наборе данных перед загрузкой новых
            if (НаборДанных.Tables.Contains("Products"))
            {
                НаборДанных.Tables["Products"].Clear();
            }

            // Заполнение набора данных данными из таблицы "Products"
            Адаптер = new MySqlDataAdapter("SELECT * FROM `Products`", Подключение);
            Адаптер.Fill(НаборДанных, "Products");

            // Установка источника данных для DataGridView
            dataGridView1.DataSource = НаборДанных;
            dataGridView1.DataMember = "Products";

            // Закрытие подключения
            Подключение.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Подготовка команд для вставки и обновления данных в таблице "Products"
            MySqlCommand insertCommand = new MySqlCommand(
                "INSERT INTO `Products` (`Product_ID`, `Product_Name`, `Material_Consumption`) VALUES (?, ?, ?)");
            insertCommand.Parameters.Add("@Product_ID", MySqlDbType.VarChar, 10, "Product_ID");
            insertCommand.Parameters.Add("@Product_Name", MySqlDbType.VarChar, 255, "Product_Name");
            insertCommand.Parameters.Add("@Material_Consumption", MySqlDbType.Decimal, 10, "Material_Consumption");

            MySqlCommand updateCommand = new MySqlCommand(
                "UPDATE `Products` SET `Product_Name` = ?, `Material_Consumption` = ? WHERE `Product_ID` = ?");
            updateCommand.Parameters.Add("@Product_Name", MySqlDbType.VarChar, 255, "Product_Name");
            updateCommand.Parameters.Add("@Material_Consumption", MySqlDbType.Decimal, 10, "Material_Consumption");
            updateCommand.Parameters.Add(new MySqlParameter("@Original_Product_ID", MySqlDbType.VarChar, 10, ParameterDirection.Input, false, (Byte)0, (Byte)0, "Product_ID", DataRowVersion.Original, null));

            // Привязка команд к адаптеру
            Адаптер.InsertCommand = insertCommand;
            Адаптер.UpdateCommand = updateCommand;

            // Установка соединения для команд
            insertCommand.Connection = Подключение;
            updateCommand.Connection = Подключение;

            try
            {
                // Обновление базы данных с использованием данных из набора данных
                var kol = Адаптер.Update(НаборДанных, "Products");
                MessageBox.Show("Обновлено " + kol.ToString() + " записей");
            }
            catch (Exception Ситуация)
            {
                // Обработка ошибок при обновлении
                MessageBox.Show(Ситуация.Message, "Недоразумение");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Проверка, выбрана ли хотя бы одна строка в DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridView1.SelectedRows[0].Index;

                // Удаление выбранной строки из набора данных
                НаборДанных.Tables["Products"].Rows[selectedIndex].Delete();

                // Подготовка команды для удаления записи из базы данных
                Команда = new MySqlCommand();
                Команда.CommandText = "DELETE FROM `Products` WHERE `Product_ID` = ?";
                Команда.Parameters.Clear();
                Команда.Parameters.Add(new MySqlParameter("Product_ID", MySqlDbType.VarChar, 10, "Product_ID"));

                // Привязка команды удаления к адаптеру
                Адаптер.DeleteCommand = Команда;
                Команда.Connection = Подключение;

                try
                {
                    // Обновление базы данных с удалением записи
                    var kol = Адаптер.Update(НаборДанных, "Products");
                    MessageBox.Show("Удалено " + kol.ToString() + " записей");
                }
                catch (Exception Ситуация)
                {
                    // Обработка ошибок при удалении
                    MessageBox.Show(Ситуация.Message, "Недоразумение");
                }
            }
            else
            {
                // Уведомление пользователя о необходимости выбора записи для удаления
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
            }
        }

    }
}