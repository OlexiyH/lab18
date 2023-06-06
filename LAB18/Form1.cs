using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace LB18
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Встановити вид елемента ListView на Details
            listView1.View = View.Details;

            // Додати два стовпці до елемента ListView
            listView1.Columns.Add("ID процесу", 100);
            listView1.Columns.Add("Назва", 150);

            // Оновити список процесів
            RefreshProcessList();
        }

        private void RefreshProcessList()
        {
            // Очистити список процесів
            listView1.Items.Clear();

            // Отримати всі запущені процеси
            Process[] processes = Process.GetProcesses();

            // Додати кожен процес до елемента ListView
            foreach (Process process in processes)
            {
                ListViewItem item = new ListViewItem(process.Id.ToString());
                item.SubItems.Add(process.ProcessName);
                listView1.Items.Add(item);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстові файли (*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        foreach (ListViewItem item in listView1.Items)
                        {
                            writer.WriteLine($"{item.Text}\t{item.SubItems[1].Text}");
                        }
                    }
                }
            }
        }

        private void listViewProcesses_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listView1.FocusedItem.Bounds.Contains(e.Location))
            {
                contextMenuProcess.Show(Cursor.Position);
            }
        }

        private void killMenuItem_Click(object sender, EventArgs e)
        {
            // Перевірити, чи вибрано процес для зупинки
            if (listView1.SelectedItems.Count > 0)
            {
                // Отримати вибраний елемент
                ListViewItem selectedProcess = listView1.SelectedItems[0];

                // Отримати ідентифікатор процесу
                int processId = Convert.ToInt32(selectedProcess.SubItems[0].Text);

                // Отримати об'єкт процесу за його ідентифікатором
                Process process = Process.GetProcessById(processId);

                // Зупинити процес
                process.Kill();

                // Оновити список процесів
                RefreshProcessList();
            }
        }
        private void infoProcess_Click(object sender, EventArgs e)
        {
            // Перевірити, чи вибрано процес для відображення інформації
            if (listView1.SelectedItems.Count > 0)
            {
                // Отримати вибраний елемент
                ListViewItem selectedProcess = listView1.SelectedItems[0];

                // Отримати ідентифікатор процесу
                int processId = Convert.ToInt32(selectedProcess.SubItems[0].Text);

                // Отримати об'єкт процесу за його ідентифікатором
                Process process = Process.GetProcessById(processId);

                // Відобразити інформацію про процес
                string processInfo = $"ID: {process.Id}\nНазва: {process.ProcessName}\n";
                processInfo += $"Загальний час роботи: {process.TotalProcessorTime}\n";
                processInfo += $"Фізична пам'ять: {process.WorkingSet64} bytes\n";
                MessageBox.Show(processInfo);
            }
        }

        // Функція для виведення інформації про потоки процесу
        private void DisplayProcessThreads(Process process)
        {
            string threadInfo = "Потоки:\n";
            foreach (ProcessThread thread in process.Threads)
            {
                threadInfo += $"ID: {thread.Id}, Статус: {thread.ThreadState}\n";
            }
            MessageBox.Show(threadInfo);
        }

        // Функція для виведення інформації про модулі процесу
        private void DisplayProcessModules(Process process)
        {
            string moduleInfo = "Модулі:\n";
            foreach (ProcessModule module in process.Modules)
            {
                moduleInfo += $"Назва: {module.ModuleName}, Версія: {module.FileVersionInfo.FileVersion}\n";
            }
            MessageBox.Show(moduleInfo);
        }

        private void infoThreadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedProcess = listView1.SelectedItems[0];
                int processId = Convert.ToInt32(selectedProcess.SubItems[0].Text);
                Process process = Process.GetProcessById(processId);
                DisplayProcessThreads(process);
            }
        }
        private void infoModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedProcess = listView1.SelectedItems[0];
                int processId = Convert.ToInt32(selectedProcess.SubItems[0].Text);
                Process process = Process.GetProcessById(processId);
                DisplayProcessModules(process);
            }
        }
    }
}
