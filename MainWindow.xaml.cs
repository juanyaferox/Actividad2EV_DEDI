using Actividad2EV.Models;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Actividad2EV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Boolean esLineas = false;
        Boolean esItin = false;
        string proyectoDirectorio;
        string filePathL;
        string filePathI;
        public MainWindow()
        {
            InitializeComponent();
            proyectoDirectorio = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            filePathL = System.IO.Path.Combine(proyectoDirectorio, "Data", "Lineas.csv");
            filePathI = System.IO.Path.Combine(proyectoDirectorio, "Data", "Paradas.csv");
        }
        private void btnLineas_Click(object sender, RoutedEventArgs e)
        {
            
            FillDataGridCSV(filePathL);
            esLineas = true;
            esItin = false;
        }
        private void btnItin_Click(object sender, RoutedEventArgs e)
        {
            string proyectoDirectorio = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            
            FillDataGridCSV(filePathI);
            esItin = true;
            esLineas = false;
        }

        private void FillDataGridCSV(String filePath)
        {
            if (File.Exists(filePath))
            {
                // Lee el archivo CSV
                DataTable dataTable = new DataTable();

                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    // Lee la primera línea del archivo CSV para obtener los nombres de las columnas
                    string[] headers = parser.ReadFields();

                    // Crea las columnas del DataTable basadas en los nombres de las columnas del CSV
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    // Lee el resto del archivo CSV y agrega las filas al DataTable
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        dataTable.Rows.Add(fields);
                    }
                }

                // Asigna el DataTable como origen de datos del DataGrid
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            else
            {
                MessageBox.Show("El archivo CSV no se encontró en la carpeta.");
            }
        }

        private void btnAlta_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBaja_Click(object sender, RoutedEventArgs e)
        {
            EliminarFilaSeleccionada();
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void EliminarFilaSeleccionada()
        {
            // Obtener la fila seleccionada
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;

            if (selectedRow != null)
            {
                // Obtener el índice de la fila seleccionada
                int selectedIndex = dataGrid.SelectedIndex;

                // Obtener el DataRow correspondiente a la fila seleccionada
                DataRowView rowView = (DataRowView)dataGrid.SelectedItem;
                DataRow row = rowView.Row;

                // Obtener el DataView asociado al origen de datos del DataGrid
                DataView dataView = (DataView)dataGrid.ItemsSource;

                // Obtener el DataTable subyacente al DataView
                DataTable dataTable = dataView.Table;

                // Eliminar la fila del DataTable
                dataTable.Rows.Remove(row);

                // Seleccionar otra fila después de eliminarla
                if (selectedIndex >= 0 && selectedIndex < dataGrid.Items.Count - 1)
                {
                    dataGrid.SelectedIndex = selectedIndex; // Seleccionar la fila siguiente
                }
                else if (selectedIndex >= 0 && selectedIndex == dataGrid.Items.Count - 1)
                {
                    dataGrid.SelectedIndex = selectedIndex - 1; // Seleccionar la fila anterior si es la última fila
                }
            }
        }


        private void btnRefres_Click(object sender, RoutedEventArgs e)
        {
            if (esLineas && !esItin)
            {
                FillDataGridCSV(filePathL);
                dataGrid.Items.Refresh();
            }
            else if (!esLineas && esItin)
            {
                FillDataGridCSV(filePathI);
                dataGrid.Items.Refresh();
            }

        }

    }
}