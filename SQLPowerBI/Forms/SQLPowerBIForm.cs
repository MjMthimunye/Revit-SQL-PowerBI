using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SQLPowerBI.Forms
{
    public partial class SQLPowerBIForm : System.Windows.Forms.Form
    {
        #region Global Variables
        Document Doc;
        SQLDBConnect sqlConnection = new SQLDBConnect();
        DataTable dgCategories = new DataTable();

        //All BuiltInCategory objects as array
        private static readonly Array valueList = Enum.GetValues(typeof(BuiltInCategory));
        private static List<BuiltInCategory> builtInCatArray = valueList.OfType<BuiltInCategory>().ToList();
        private static readonly HashSet<string> checkedRowsString = new HashSet<string>();
        #endregion

        public SQLPowerBIForm(Document doc)
        {
            InitializeComponent();
            Doc = doc;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {

            Dictionary<string, Dictionary<string, List<string>>> map_cat_to_uid_to_param_values
             = new Dictionary<string, Dictionary<string, List<string>>>();

            search_text.Text = "";

            //Get the project name from the document title
            string projectName = Doc.Title;

            //get param data from selected category 
            foreach (DataGridViewRow row in dataGrid_Categories.Rows)
            {

                string selectedCategory = row.Cells["Categories"].Value.ToString();

                foreach(string value in checkedRowsString)
                {
                    if(value == selectedCategory)
                    {

                        foreach (BuiltInCategory bic in builtInCatArray)
                        {
                            //Get category name with OST prefix removed
                            var cat = bic.ToString().Substring(bic.ToString().IndexOf("_") + 1);

                            if (cat == selectedCategory)
                            {
                                //Category variable used to identify project and selected category
                                string category = projectName + "_" + cat;

                                category = string.Join("", category.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                                category = Regex.Replace(category, "[0-9]", "");

                                if (category.Contains("-"))
                                {
                                    category = category.Replace("-", "_");
                                }

                                //Remove unwanted characters from project name
                                if (category.Contains("~"))
                                {
                                    category = category.Replace("~", "_");
                                }
                                if (category.Contains("["))
                                {
                                    category = category.Replace("[", string.Empty);
                                }
                                if (category.Contains("]"))
                                {
                                    category = category.Replace("]", string.Empty);
                                }
                                if (category.Contains("{"))
                                {
                                    category = category.Replace("{", string.Empty);
                                }
                                if (category.Contains("}"))
                                {
                                    category = category.Replace("}", string.Empty);
                                }
                                if (category.Contains("("))
                                {
                                    category = category.Replace("(", string.Empty);
                                }
                                if (category.Contains(")"))
                                {
                                    category = category.Replace(")", string.Empty);
                                }


                                map_cat_to_uid_to_param_values.Add(category, new Dictionary<string, List<string>>());

                                List<ElementId> bicID = new List<ElementId>();
                                bicID.Add(new ElementId((int)bic));

                                IList<ElementFilter> a = new List<ElementFilter>();
                                a.Add(new ElementCategoryFilter(bic));

                                var categoryFilter = new LogicalOrFilter(a);

                                //Apply the filter to the elements in the active document
                                ElementCategoryFilter filter = new ElementCategoryFilter(bic);

                                //Run the collector
                                var els = new FilteredElementCollector(Doc)
                                        .WhereElementIsNotElementType()
                                        .WhereElementIsViewIndependent()
                                        .WherePasses(categoryFilter);

                                List<string> paramListParams = new List<string>();

                                //Check if table is created if so just save values.
                                bool doesExist = TableExists("Jacobian", category);

                                //If table exists , drop the table and reacreate it with new data
                                //This is a way to replace the old data
                                if(doesExist)
                                {
                                    SqlCommand command = sqlConnection.Query("DROP TABLE " + category);
                                    command.ExecuteNonQuery();
                                }

                                foreach (Element ele in els)
                                {
                                    var elementCat = ele.Category;
                                    if (null == elementCat)
                                    {
                                        Debug.Print("element {0} {1} has null category", ele.Id, ele.Name);
                                        continue;
                                    }

                                    //Gets all the paramaters with extra family type and type id, etc.
                                    List<string> param_values = GetParamValues(ele);

                                    //Add family name to param values
                                    var FamilyName = "FamilyName = " + ele.Name;
                                    param_values.Add(FamilyName);

                                    //Add element id name to param values
                                    var ElementId = "ElementId = " + ele.Id.IntegerValue;
                                    param_values.Add(ElementId);

                                    string uid = ele.UniqueId;

                                    //Add uid, param_values and the category to map_cat_to_uid_to_param_values 
                                    map_cat_to_uid_to_param_values[category].Add(uid, param_values);

                                    //Push params to list to get distict list
                                    for (var i = 0; i < param_values.Count; i++)
                                    {
                                        List<string> elementParams = new List<string>(param_values[i].Split(new string[] { " = " }, StringSplitOptions.None));
                                        string elementParam = string.Join("", elementParams[0].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                                        //\\//\\Refactor if statement code//\\//\\
                                        //\\//\\//\\//\\//\\//\\//\\//\\//\\//\\
                                        if (elementParam.Contains("-"))
                                        {
                                            elementParam = elementParam.Replace("-", "_");
                                        }

                                        if (elementParam.Contains("/"))
                                        {
                                            elementParam = elementParam.Replace("/", "_");
                                        }

                                        if (elementParam.Contains("."))
                                        {
                                            elementParam = elementParam.Replace(".", "_");
                                        }

                                        if (elementParam.Contains(":"))
                                        {
                                            elementParam = elementParam.Replace(":", "_");
                                        }

                                        if (elementParam.Contains("ON"))
                                        {
                                            elementParam = elementParam.Replace("ON", "on" + "_" + cat);
                                        }
                                        
                                        if (elementParam.Contains("Select"))
                                        {
                                            elementParam = elementParam.Replace("Select", "Select" + "_" + cat);
                                        }

                                        if (elementParam.Contains("Insert"))
                                        {
                                            elementParam = elementParam.Replace("Insert", "Insert" + "_" + cat);
                                        }

                                        if (elementParam.Contains("Drop"))
                                        {
                                            elementParam = elementParam.Replace("Drop", "Drop" + "_" + cat);
                                        }

                                        if (elementParam.Contains("Create"))
                                        {
                                            elementParam = elementParam.Replace("Create", "Create" + "_" + cat);
                                        }

                                        if (elementParam.Contains("Into"))
                                        {
                                            elementParam = elementParam.Replace("Into", "Into" + "_" + cat);
                                        }

                                        if (elementParam.Contains("Table"))
                                        {
                                            elementParam = elementParam.Replace("Table", "Table" + "_" + cat);
                                        }

                                        paramListParams.Add(elementParam);

                                    }

                                }

                                //Get distinct parameters from paramListParams
                                List<string> distictParams = paramListParams.Distinct().ToList();

                                //Declare string builders 
                                StringBuilder sqlCreateTable = new StringBuilder("CREATE TABLE " + category + "(UniqueId varchar(255) NOT NULL PRIMARY KEY");
                                StringBuilder sqlSaveData = new StringBuilder("INSERT INTO " + category + "(UniqueId");
                                StringBuilder sqlSaveDataValues = new StringBuilder("VALUES" + "(@param1");

                                for (var i = 0; i < distictParams.Count; i++)
                                {
                                    if (i == distictParams.Count - 1)
                                    {

                                        sqlCreateTable.Append(", " + distictParams[i] + " varchar(255))");
                                        sqlSaveData.Append(", " + distictParams[i] + ") ");
                                        sqlSaveDataValues.Append($", @param{i + 2})");

                                    }
                                    else
                                    {
                                        sqlCreateTable.Append(", " + distictParams[i] + " varchar(255)");
                                        sqlSaveData.Append(", " + distictParams[i]);
                                        sqlSaveDataValues.Append($", @param{i + 2}");

                                    }
                                }

                                //table and set query strings
                                string tableQuery = sqlCreateTable.ToString();
                                string setQuery = sqlSaveData.Append(sqlSaveDataValues.ToString()).ToString();

                                foreach (var dictPair in map_cat_to_uid_to_param_values)
                                {
                                    if (dictPair.Value.Count > 0)
                                    {
                                        foreach (var innerPair in dictPair.Value)
                                        {
                                            string uniqueId = innerPair.Key;
                                            List<string> paramList = innerPair.Value;

                                            //Check if table is created if so just save values.
                                            doesExist = TableExists("Jacobian", category);

                                            if (doesExist)
                                            {
                                                Debug.WriteLine(category + "table already exists");

                                                try
                                                {
                                                    SaveParamsSQL(paramList, setQuery, uniqueId, distictParams);
                                                }
                                                catch (Exception ex)
                                                {

                                                    MessageBox.Show(ex.ToString());
                                                }

                                            }
                                            else
                                            {
                                                try
                                                {
                                                    //Create sql table
                                                    SqlCommand command = sqlConnection.Query(tableQuery);
                                                    command.ExecuteNonQuery();

                                                    SaveParamsSQL(paramList, setQuery, uniqueId, distictParams);

                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show(ex.ToString());
                                                }

                                            }

                                        }

                                    }
                                    else
                                    {
                                        //MessageBox.Show("No data was found!");
                                        Debug.Print("No data was found!");

                                    }

                                }

                                map_cat_to_uid_to_param_values.Clear();



                            }
                        }
                    }
                }

            }

            MessageBox.Show("Data successfully exported");
            checkedRowsString.Clear();
            DialogResult = DialogResult.OK;
            Close();

        } 

        private void SQLPowerBIForm_Load(object sender, EventArgs e)
        {
            //Connect to sql database
            sqlConnection.ConnectDB();

            this.BindData();

        }

        /// <summary>
        /// Get Revit categories and bind to datagrid as form loads
        /// </summary>
        private void BindData()
        {
            //Remove unwanted categories from catagories datagrid
            builtInCatArray.RemoveAll(item => item.ToString().Contains("Deprecated"));
            builtInCatArray.RemoveAll(item => item.ToString().Contains("Obsolete"));
            builtInCatArray.RemoveAll(item => item.ToString().Contains("HiddenLines"));
            builtInCatArray.RemoveAll(item => item.ToString() == "INVALID");

            if (dgCategories.Columns.Count == 0)
            {
                //Add Categories column
                dgCategories.Columns.Add(new DataColumn("Categories", typeof(string)));
            }

            foreach (BuiltInCategory bic in builtInCatArray)
            {
                var category = bic.ToString().Substring(bic.ToString().IndexOf("_") + 1);
                dgCategories.Rows.Add(category);
            }

            dataGrid_Categories.DataSource = dgCategories;

            //Add a CheckBox Column to the categories DataGridView at the first position.
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.HeaderText = "";
            checkBoxColumn.Width = 30;
            checkBoxColumn.Name = "checkBoxColumn";
            dataGrid_Categories.Columns.Insert(0, checkBoxColumn);

            //Change the AutoSize mode to fill the width of the DataGridView as it resizes
            dataGrid_Categories.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGrid_Categories.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //dataGrid_Categories.BackgroundColor = dataGrid_Categories.DefaultCellStyle.BackColor;
            //Sort list in ascending order
            dataGrid_Categories.Sort(dataGrid_Categories.Columns["Categories"], ListSortDirection.Ascending);

        }

        /// <summary>
        /// Check if SQL table exists
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool TableExists(string database, string name)
        {
            try
            {
                //SQL query to check if table exists
                string existsQuery = "select case when exists((select * FROM [" + database + "].sys.tables " +
                    "WHERE name = '" + name + "')) then 1 else 0 end";

                SqlCommand command = sqlConnection.Query(existsQuery);

                //If value is 1 table exists if 0 , table doesnt exist
                return (int)command.ExecuteScalar() == 1;
            }
            catch (Exception err)
            {

                TaskDialog.Show("Error", err.ToString());
                return true;
            }
        }

        /// <summary>
        /// Save parameters to SQL
        /// </summary>
        /// <param name="paramList"></param>
        /// <param name="setQuery"></param>
        /// <param name="uniqueId"></param>
        /// <param name="distictParams"></param>
        private void SaveParamsSQL(List<string> paramList, string setQuery, string uniqueId, List<string> distictParams)
        {
            SqlCommand command = sqlConnection.Query(setQuery);
            command.Parameters.AddWithValue($"@param1", uniqueId);

            //parameters from element 
            List<string> paramListParams = new List<string>();

            //get all params from wall
            for (var i = 0; i < paramList.Count; i++)
            {
                List<string> elementParams = new List<string>(paramList[i].Split(new string[] { " = " }, StringSplitOptions.None));
                string elementParam = string.Join("", elementParams[0].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                if (elementParam.Contains("-"))
                {
                    elementParam = elementParam.Replace("-", string.Empty);

                }

                paramListParams.Add(elementParam);

            }

            //remove duplicates in param list if any, try hash set or distinct instead of iterating.
            for (int i = 0; i < paramList.Count - 1; i++)
            {
                for (int j = i + 1; j < paramList.Count; j++)
                {

                    List<string> elementParams = new List<string>(paramList[i].Split(new string[] { " = " }, StringSplitOptions.None));
                    string elementParam = string.Join("", elementParams[0].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                    List<string> elementParamsJ = new List<string>(paramList[j].Split(new string[] { " = " }, StringSplitOptions.None));
                    string elementParamJ = string.Join("", elementParamsJ[0].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                    if (elementParam == elementParamJ)
                    {
                        paramList.RemoveAt(j);
                        paramListParams.RemoveAt(j);
                    }

                }
            }

            //Loop through all distinct parameters
            for (var i = 0; i < distictParams.Count; i++)
            {
                //Get the index of the paramListParams parameter that is equal to the distinct Parameter 
                int index = paramListParams.FindIndex(a => a == distictParams[i]);

                if (index >= 0)
                {
                    try
                    {
                        List<string> elementParams = new List<string>(paramList[index].Split(new string[] { " = " }, StringSplitOptions.None));
                        string elementParamValue = string.Join("", elementParams[1].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                        //Remove unwanted parameters
                        switch (elementParamValue)
                        {
                            case var s when elementParamValue.Contains("°"):
                                elementParamValue = elementParamValue.Replace("°", string.Empty);
                                break;
                            case var s when elementParamValue.Contains("m³"):
                                elementParamValue = elementParamValue.Replace("m³", string.Empty);
                                break;
                            case var s when elementParamValue.Contains("<None>"):
                                elementParamValue = elementParamValue.Replace("<None>", string.Empty);
                                break;
                            case var s when elementParamValue.Contains("m²"):
                                elementParamValue = elementParamValue.Replace("m²", string.Empty);
                                break;
                        }

                        command.Parameters.AddWithValue($"@param{i + 2}", elementParamValue);

                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else
                {
                    try
                    {
                        command.Parameters.AddWithValue($"@param{i + 2}", "");
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }           
     
            }
           
            command.ExecuteNonQuery();
        }

        /// <summary>
        ///     Return all the parameter values
        ///     deemed relevant for the given element
        ///     in string form.
        /// </summary>
        private static List<string> GetParamValues(Element e)
        {
            // Two choices: 
            // Element.Parameters property -- Retrieves 
            // a set containing all the parameters.
            // GetOrderedParameters method -- Gets the 
            // visible parameters in order.

            var ps = e.GetOrderedParameters();
            var param_values = new List<string>(ps.Count);

            foreach (var p in ps)
                // AsValueString displays the value as the 
                // user sees it. In some cases, the underlying
                // database value returned by AsInteger, AsDouble,
                // etc., may be more relevant.

                param_values.Add($"{p.Definition.Name} = {p.AsValueString()}");
            return param_values;
        }

        /// <summary>
        /// Filter for searched category 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void search_text_TextChanged(object sender, EventArgs e)
        {
            //Filter for searched category
            DataView dv = dgCategories.DefaultView;
            dv.RowFilter = "Categories LIKE '" + search_text.Text + "%'";

            dataGrid_Categories.DataSource = dv;

            //Maintain cell checked state even if search changes
            foreach (DataGridViewRow row in dataGrid_Categories.Rows)
            {
                string selectedCategory = row.Cells["Categories"].Value.ToString();

                foreach (string value in checkedRowsString)
                {
                    if (value == selectedCategory)
                    {

                        row.Cells["checkBoxColumn"].Value = true;
                    }

                }
            }

        }

        /// <summary>
        /// Save checked rows to checkedRowsString Hashset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_Categories_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow row in dataGrid_Categories.Rows)
            {
                bool isChecked = Convert.ToBoolean(row.Cells["checkBoxColumn"].Value);

                if (isChecked)
                {
                    string selectedCategory = row.Cells["Categories"].Value.ToString();

                    checkedRowsString.Add(selectedCategory);
                
                }

            }

        }


    }
}
