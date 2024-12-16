using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace БдUpdate
{
    public partial class ReportForm : Form
    {
        private DataSet НаборДанных;

        public ReportForm(DataSet наборДанных)
        {
            InitializeComponent();
            НаборДанных = наборДанных;
            this.Load += new EventHandler(ReportForm_Load); // Подписка на событие загрузки формы
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            GenerateReport(); // Генерация отчета при загрузке формы
        }

        private void GenerateReport()
        {
            if (НаборДанных.Tables.Contains("Products"))
            {
                DataTable productsTable = НаборДанных.Tables["Products"];
                decimal totalAmount = 0;

                // Настройка DataGridView
                dataGridView2.Columns.Clear();
                dataGridView2.Columns.Add("Product_ID", "Номер продукта");
                dataGridView2.Columns.Add("Product_Name", "Название");
                dataGridView2.Columns.Add("Material_Consumption", "Размер");

                foreach (DataRow row in productsTable.Rows)
                {
                    string productId = row["Product_ID"].ToString();
                    string productName = row["Product_Name"].ToString();
                    decimal materialConsumption = Convert.ToDecimal(row["Material_Consumption"]);

                    // Добавление строки в DataGridView
                    dataGridView2.Rows.Add(productId, productName, materialConsumption);
                    totalAmount += materialConsumption; // Суммируем материал
                }

                // Добавление итоговой строки
                int totalCount = productsTable.Rows.Count;
                dataGridView2.Rows.Add("", "Итого", totalAmount);
                dataGridView2.Rows.Add("", "Количество", totalCount);
            }
            else
            {
                MessageBox.Show("Нет данных для отображения.");
            }
        }

        private void buttonSendReport_Click(object sender, EventArgs e)
        {
            SendReport(); // Отправка отчета при нажатии кнопки
        }

        private void SendReport()
        {
            try
            {
                // Создание сообщения
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("your_email@example.com"); // Укажите ваш адрес электронной почты
                mail.To.Add("recipient@example.com"); // Укажите адрес получателя
                mail.Subject = "Отчет о продуктах";
                mail.Body = GenerateReportBody(); // Метод для генерации текста отчета

                // Настройка SMTP-клиента
                SmtpClient smtp = new SmtpClient("smtp.example.com", 587); // Укажите SMTP-сервер и порт
                smtp.Credentials = new NetworkCredential("your_email@example.com", "your_password"); // Укажите учетные данные
                smtp.EnableSsl = true; // Включите SSL, если требуется

                // Отправка сообщения
                smtp.Send(mail);
                MessageBox.Show("Отчет успешно отправлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке отчета: " + ex.Message);
            }
        }

        private string GenerateReportBody()
        {
            // Генерация текста отчета
            if (НаборДанных.Tables.Contains("Products"))
            {
                DataTable productsTable = НаборДанных.Tables["Products"];
                string report = "Номер продукта - Название - Размер\n";

                foreach (DataRow row in productsTable.Rows)
                {
                    string productId = row["Product_ID"].ToString();
                    string productName = row["Product_Name"].ToString();
                    string materialConsumption = row["Material_Consumption"].ToString();

                    report += $"{productId} - {productName} - {materialConsumption}\n";
                }

                return report;
            }
            return "Нет данных для отчета.";
        }
    }
}