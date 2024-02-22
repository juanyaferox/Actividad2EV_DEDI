using Actividad2EV.Models;
using Microsoft.VisualBasic.FileIO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
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
            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           
            if (e.Key == Key.F1)
            {
                VentanaAyuda();
            }
        }
        private void btnLineas_Click(object sender, RoutedEventArgs e)
        {
            esLineas = true;
            esItin = false;
            FillDataGridCSV(filePathL);
            
        }
        private void btnItin_Click(object sender, RoutedEventArgs e)
        {
            esItin = true;
            esLineas = false;
            FillDataGridCSV(filePathI);
            
        }

        private void FillDataGridCSV(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("El archivo CSV no se encontró en la carpeta.");
                return;
            }

            List<object> dataList = new List<object>();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");

                // Descarta las primeras líneas al ser cabeceras
                parser.ReadLine();

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    if (esLineas && !esItin)
                    {
                        AddLineaToList(fields, dataList);
                    }
                    else if (!esLineas && esItin)
                    {
                        AddParadasToList(fields, dataList);
                    }
                }
            }
            dataGrid.ItemsSource = dataList;
        }
        private void AddLineaToList(string[] fields, List<object> dataList)
        {
            Linea linea = ConstruirLineaDesdeCSV(fields);
            dataList.Add(linea);
        }

        private void AddParadasToList(string[] fields, List<object> dataList)
        {
            Paradas paradas = ConstruirParadasDesdeCSV(fields);
            dataList.Add(paradas);
        }
        private Linea ConstruirLineaDesdeCSV(string[] fields)
        {
            return new Linea
            {
                Id = Convert.ToInt32(fields[0]),
                MunicipioOr = fields[1],
                MunicipioDest = fields[2],
                HoraInic = fields[3],
                IntervaloBus = fields[4]
            };
        }

        private Paradas ConstruirParadasDesdeCSV(string[] fields)
        {
            return new Paradas
            {
                NumLinea = Convert.ToInt32(fields[0]),
                Municipio = fields[1],
                IntervaloHS = fields[2]
            };
        }
        private void btnAlta_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.CanUserAddRows = true;
            dataGrid.IsReadOnly = false;
            dataGrid.BeginningEdit += dataGrid_BeginningEdit;
        }

        private void btnBaja_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EliminarFilaSeleccionada();
            }
            catch (InvalidCastException ex)
            {
                ex.Data.Add("MensajeAdicional", "Hiciste algo ilegal amigo");
            }
            dataGrid.IsReadOnly = true;
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.CanUserAddRows = false;
            dataGrid.IsReadOnly = false;
            dataGrid.BeginningEdit -= dataGrid_BeginningEdit;
        }
        private void EliminarFilaSeleccionada()
        {
            if (esLineas)
            {
                Linea selectedLinea = (Linea)dataGrid.SelectedItem;

                if (selectedLinea != null)
                {
                    
                    List<object> items = (List<object>)dataGrid.ItemsSource;
                    items.Remove(selectedLinea);
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = items;
                }
            }
            else if (esItin)
            {
                Paradas selectedParada = (Paradas)dataGrid.SelectedItem;

                if (selectedParada != null)
                {
                    // Obtener la lista actual del origen de datos y convertirla a List<Linea>
                    List<object> items = (List<object>)dataGrid.ItemsSource;
                    items.Remove(selectedParada);
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = items;
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
        
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {//Ni idea de como funciona esto copia y pega de stackoverflow pero funciona adioosss
            var displayName = GetPropertyDisplayName(e.PropertyDescriptor);

            if (!string.IsNullOrEmpty(displayName))
            {
                e.Column.Header = displayName;
            }

        }

        public static string GetPropertyDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;

            if (pd != null)
            {
                // Check for DisplayName attribute and set the column header accordingly
                var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                if (displayName != null && displayName != DisplayNameAttribute.Default)
                {
                    return displayName.DisplayName;
                }

            }
            else
            {
                var pi = descriptor as PropertyInfo;

                if (pi != null)
                {
                    // Check for DisplayName attribute and set the column header accordingly
                    Object[] attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    for (int i = 0; i < attributes.Length; ++i)
                    {
                        var displayName = attributes[i] as DisplayNameAttribute;
                        if (displayName != null && displayName != DisplayNameAttribute.Default)
                        {
                            return displayName.DisplayName;
                        }
                    }
                }
            }

            return null;
        }
        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // Verifica si la edición se está realizando en una fila existente
            if (e.Row != null && !e.Row.IsNewItem)
            {
                // Cancela la edición
                e.Cancel = true;
            }
        }
        private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            // Verifica si la celda pertenece a la primera columna
            if (e.Column != null && e.Column.DisplayIndex == 0)
            {
                // Accede a la celda y establece IsReadOnly en true para la primera columna
                DataGridCell cell = e.Column.GetCellContent(e.Row).Parent as DataGridCell;
                if (cell != null)
                {
                    cell.IsEnabled = false;
                    cell.Focusable = false;
                }
            }
        }
        private void VentanaAyuda()
        {
            string texto = "Gestión de Lineas - Muestra la información de las lineas\nGestión de itinerarios - Muestra la información de los itinerarios" +
                    "\nAlta - Habilita la posibilidad de añadir nueva información a la base de datos seleccionada\nBaja - Borrar la fila seleccionada" +
                    "\nModificar - Habilita la modificación de los datos ya existente de la base de datos seleccionado\nRefrescar - Recarga la base de datos" +
                    "\nConfirmar cambios - Guarda en la base de datos las modificaciones realizadas";
            MessageBox.Show(texto, "Ayuda", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void btnAyuda_Click(object sender, RoutedEventArgs e)
        {
            VentanaAyuda();
        }
    }
}