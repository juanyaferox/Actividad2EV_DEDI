using Actividad2EV.Models;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Actividad2EV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Boolean esLineas;
        Boolean esItin;
        string proyectoDirectorio;
        string filePathL;
        string filePathI;
        List<object> dataList = new List<object>();
        List<object> backupListL = new List<object>();
        List<object> backupListP = new List<object>();
        Boolean backupL;
        Boolean backupP;
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
        //no es coña q el tema d botones fue lo más dolor d cabeza me dió
        private void btnLineas_Click(object sender, RoutedEventArgs e)
        {
            if (esLineas)
            {
                return;
            } if (esItin)
            {

                backupListP = dataList.ToList();
                backupP = true;
                
                //MessageBox.Show("Respaldo paradas\n" + string.Join(", ", backupListP));
            }
            esLineas = true;
            esItin = false;
            if (backupL)
            {

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = backupListL;
                backupL = false;
                //MessageBox.Show("Estás en bkacupL:\n"+ string.Join(", ", backupListL));
            }
            else
            {
                FillDataGridCSV(filePathL);

            }

        }
        private void btnItin_Click(object sender, RoutedEventArgs e)
        {
            if (esItin)
            {
                return;
            }
            if (esLineas)
            {
                backupListL = dataList.ToList();
                backupL = true;
                //MessageBox.Show("Respaldo lista:\n" + string.Join(", ", backupListL));
            }
            esItin = true;
            esLineas = false;
            if (backupP)
            {

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = backupListP;
                
                backupP = false;
                //MessageBox.Show("Estás en bkacupP:\n" + string.Join(", ", backupListP));
            }
            else
            {
                FillDataGridCSV(filePathI);

            }

        }

        private void FillDataGridCSV(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("El archivo CSV no se encontró en la carpeta.");
                return;
            }
            dataList.Clear();
            
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                
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
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = dataList;
            //MessageBox.Show("Este es el data list:\n" + string.Join(", ", dataList));
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
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int columnIndex = e.Column.DisplayIndex;
            TextBox editedTextBox = e.EditingElement as TextBox;
            string newValue = editedTextBox.Text;
            object editedObject = e.Row.Item;
            

            if (esLineas)
            {
                Linea editedLinea = editedObject as Linea;
                if (editedLinea != null)
                {
                    switch (columnIndex)
                    {
                        case 1:
                            editedLinea.MunicipioOr = newValue;
                            break;
                        case 2:
                            editedLinea.MunicipioDest = newValue;
                            break;
                        case 3:
                            editedLinea.HoraInic = newValue;
                            break;
                        case 4:
                            editedLinea.IntervaloBus = newValue;
                            break;
                    }
                    int index = dataList.IndexOf(editedLinea);
                    if (index != -1)
                    {
                        dataList[index] = editedLinea;
                    }
                }
            }
            else if (esItin)
            {
                Paradas editedLinea = editedObject as Paradas;
                if (editedLinea != null)
                {
                    switch (columnIndex)
                    {
                        case 0:
                            editedLinea.NumLinea = int.Parse(newValue);
                            break;
                        case 1:
                            editedLinea.Municipio = newValue;
                            break;
                        case 2:
                            editedLinea.IntervaloHS = newValue;
                            break;
                    }
                    int index = dataList.IndexOf(editedLinea);
                    if (index != -1)
                    {
                        dataList[index] = editedLinea;
                    }
                }
            }
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
            
            if (e.Row != null && !e.Row.IsNewItem)
            {
               
                e.Cancel = true;
            }
        }
        private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.Column != null && e.Column.DisplayIndex == 0)
            {
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

        private void btnConf_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("¿Estás seguro de realizar esta acción?", "Confirmación", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                if (esLineas)
                {
                    List<object> lineasList = new List<object>();
                    foreach (Linea linea in dataGrid.Items)
                    {
                        lineasList.Add(linea);
                    }
                    SaveListToCSV(lineasList, filePathL);
                    MessageBox.Show("Realiado con éxito");
                }
                else if (esItin)
                {
                    List<object> paradasList = new List<object>();
                    foreach (Paradas parada in dataGrid.Items)
                    {
                        paradasList.Add(parada);
                    }
                    SaveListToCSV(paradasList, filePathI);
                    MessageBox.Show("Realiado con éxito");
                }
            }
        }
        public void SaveListToCSV(List<object> dataList, string filePath)
        {
            // Crear un StringBuilder para construir el contenido del archivo CSV
            StringBuilder csvContent = new StringBuilder();

            // Obtener las propiedades del primer objeto en la lista
            var firstItem = dataList.FirstOrDefault();
            if (firstItem != null)
            {
                var properties = firstItem.GetType().GetProperties();

                // Obtener los nombres de las propiedades y escribirlos como encabezados en la primera línea del CSV
                var headers = string.Join(";", properties.Select(p => p.Name));
                csvContent.AppendLine(headers);

                // Iterar sobre cada elemento de la lista y agregar sus valores al contenido CSV
                foreach (var item in dataList)
                {
                    // Obtener los valores de las propiedades del objeto y unirlos con punto y coma (;)
                    var values = string.Join(";", properties.Select(p => p.GetValue(item)?.ToString() ?? ""));

                    // Agregar los valores al contenido CSV con un salto de línea al final
                    csvContent.AppendLine(values);
                }
            }

            // Escribir el contenido del archivo CSV en el archivo especificado
            File.WriteAllText(filePath, csvContent.ToString());
        }

    }
}