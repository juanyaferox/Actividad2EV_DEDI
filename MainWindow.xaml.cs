using Actividad2EV.Models;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        bool esLineas;
        bool esItin;
        bool alta;
        bool baja;
        bool mod;
        string proyectoDirectorio;
        string filePathL;
        string filePathI;
        string filePathM;
        List<object> dataListLinea = new List<object>();
        List<object> dataListParada = new List<object>();
        public MainWindow()
        {
            InitializeComponent();
            proyectoDirectorio = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            filePathL = System.IO.Path.Combine(proyectoDirectorio, "Data", "Lineas.csv");
            filePathI = System.IO.Path.Combine(proyectoDirectorio, "Data", "Paradas.csv");
            filePathM = System.IO.Path.Combine(proyectoDirectorio, "Data", "Municipios .csv");
            PreviewKeyDown += MainWindow_PreviewKeyDown;
            esLineas = true;
            FillDataGridCSV(filePathL);
            esItin = true;
            esLineas = false;
            FillDataGridCSV(filePathI);
            esItin = false;
            //un poco liada aqui, pero es q al principio hice un solo metodo que me rellenase una misma lista y la fuera borrando cuando accedia al otro
            //y para no volver a remodelar hice esta aberración
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
            }
            esLineas = true;
            esItin = false;
            if (mod)
            {
                dataGrid.PreparingCellForEdit += dataGrid_PreparingCellForEdit;
            } else if (alta)
            {
                dataGrid.PreparingCellForEdit += dataGrid_PreparingCellForEdit;
            }
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = dataListLinea;

        }
        private void btnItin_Click(object sender, RoutedEventArgs e)
        {
            if (esItin)
            {
                return;
            }
            esItin = true;
            esLineas = false;
            if (mod)
            {
                dataGrid.PreparingCellForEdit -= dataGrid_PreparingCellForEdit;
            } else if (alta)
            {
                dataGrid.PreparingCellForEdit -= dataGrid_PreparingCellForEdit;
            }
            dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = dataListParada;
        }


        private void btnAlta_Click(object sender, RoutedEventArgs e)
        {
            alta = true;
            baja = false;
            mod = false;
            dataGrid.CanUserAddRows = true;
            dataGrid.IsReadOnly = false;
            dataGrid.BeginningEdit += dataGrid_BeginningEdit;
            if (esItin)
            {
                dataGrid.PreparingCellForEdit -= dataGrid_PreparingCellForEdit;
            }
            else
            {
                dataGrid.PreparingCellForEdit += dataGrid_PreparingCellForEdit;
            }
        }

            private void btnBaja_Click(object sender, RoutedEventArgs e)
        {
            alta = false;
            baja = true;
            mod = false;
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
            alta = false;
            baja = false;
            mod = true;
            dataGrid.CanUserAddRows = false;
            dataGrid.IsReadOnly = false;
            dataGrid.BeginningEdit -= dataGrid_BeginningEdit;
            if (esItin)
            {
                dataGrid.PreparingCellForEdit -= dataGrid_PreparingCellForEdit;
            } else
            {
                dataGrid.PreparingCellForEdit += dataGrid_PreparingCellForEdit;
            }
            

        }
        private void FillDataGridCSV(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("El archivo CSV no se encontró en la carpeta.");
                return;
            }
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
                        AddLineaToList(fields, dataListLinea);
                    }
                    else if (!esLineas && esItin)
                    {
                        AddParadasToList(fields, dataListParada);
                    }
                }
            }
        }
        private void AddLineaToList(string[] fields, List<object> dataListLinea)
        {
            Linea linea = ConstruirLineaDesdeCSV(fields);
            dataListLinea.Add(linea);
        }

        private void AddParadasToList(string[] fields, List<object> dataListParada)
        {
            Paradas paradas = ConstruirParadasDesdeCSV(fields);
            dataListParada.Add(paradas);
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
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int columnIndex = e.Column.DisplayIndex;
            TextBox editedTextBox = e.EditingElement as TextBox;
            string newValue = editedTextBox.Text;
            object editedObject = e.Row.Item;
            

            if (esLineas)
            {
                Linea editedLinea = editedObject as Linea;
                bool coincide = VerificarCoincidenciaCSV(filePathM, newValue);
                bool esFormatoValido = VerificarFormatoHora(newValue);
                if (editedLinea != null)
                {
                    switch (columnIndex)
                    {

                        case 1:
                            if (coincide)
                            {
                                editedLinea.MunicipioOr = newValue;
                                break;
                            }
                            else
                            {
                                MessageBox.Show("Error: No es un municipio válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                e.Cancel = true;
                                return;
                            }
                        case 2:
                            if (coincide)
                            {
                                editedLinea.MunicipioDest = newValue;
                                break;
                            }
                            else
                            {
                                MessageBox.Show("Error: No es un municipio válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                e.Cancel = true;
                                return;
                            }
                        case 3:
                            
                            if (esFormatoValido)
                            {
                                editedLinea.HoraInic = newValue;
                                break;
                            } else
                            {
                                MessageBox.Show("Error: El valor introducido no se trata de una hora.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                e.Cancel = true;
                                return;
                            }
                        case 4:
                            if (esFormatoValido)
                            {
                                editedLinea.IntervaloBus = newValue;
                                break;
                            }
                            else
                            {
                                MessageBox.Show("Error: El valor introducido no se trata de una hora.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                e.Cancel = true;
                                return;
                            }
                    }
                    int index = dataListLinea.IndexOf(editedLinea);
                    if (index != -1)
                    {
                        dataListLinea[index] = editedLinea;
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
                            try
                            {
                                editedLinea.NumLinea = int.Parse(newValue);
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error: El valor introducido no se trata de un número entero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                e.Cancel = true;
                                return;
                            }
                            break;
                        case 1:
                            bool coincide = VerificarCoincidenciaCSV(filePathM, newValue);
                            if (coincide)
                            {
                                editedLinea.Municipio = newValue;
                                break;
                            } else
                            {
                                MessageBox.Show("Error: No es un municipio válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                e.Cancel = true;
                                return;
                            }
                            
                        case 2:
                            bool esFormatoValido = VerificarFormatoHora(newValue);
                            if (esFormatoValido)
                            {
                                editedLinea.IntervaloHS = newValue;
                                break;
                            }
                            else
                            {
                                MessageBox.Show("Error: El valor introducido no se trata de una hora.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                e.Cancel = true;
                                return;
                            }
                    }
                    int index = dataListParada.IndexOf(editedLinea);
                    if (index != -1)
                    {
                        dataListParada[index] = editedLinea;
                    }
                }
            }
        }
        public static bool VerificarFormatoHora(string hora)
        {

            string patron = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$";
            return Regex.IsMatch(hora, patron);
        }
        public static bool VerificarCoincidenciaCSV(string filePath, string input)
        {
            string[] lines = File.ReadAllLines(filePath);            
            for (int i = 1; i < lines.Length; i++)
            {
                string[] columns = lines[i].Split(';');
                if (columns.Length > 1 && columns[1].Trim().Equals(input.Trim(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
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
            
            StringBuilder csvContent = new StringBuilder();

            
            var firstItem = dataList.FirstOrDefault();
            if (firstItem != null)
            {
                var properties = firstItem.GetType().GetProperties();

               
                var headers = string.Join(";", properties.Select(p => p.Name));
                csvContent.AppendLine(headers);

                
                foreach (var item in dataList)
                {
                   
                    var values = string.Join(";", properties.Select(p => p.GetValue(item)?.ToString() ?? ""));

                   
                    csvContent.AppendLine(values);
                }
            }

           
            File.WriteAllText(filePath, csvContent.ToString());
        }

        private void dataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            if (esLineas) 
            {
                int newId = GetLastIdFromGrid() + 1;


                var newItem = new Linea();
                newItem.Id = newId;

                e.NewItem = newItem;
            }
            
        }
        private int GetLastIdFromGrid()
        {
            int lastId = 0;

            foreach (var item in dataGrid.Items)
            {
                if (item is Linea) 
                {
                    var currentItem = item as Linea;
                    lastId = Math.Max(lastId, currentItem.Id);
                }
            }
            return lastId;
        }

    }
}