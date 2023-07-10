namespace SQLPowerBI.Forms
{
    partial class SQLPowerBIForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SQLPowerBIForm));
            this.btn_save = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.search_text = new System.Windows.Forms.TextBox();
            this.dataGrid_Categories = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid_Categories)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(18, 644);
            this.btn_save.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(493, 47);
            this.btn_save.TabIndex = 1;
            this.btn_save.Text = "Export Data";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 16);
            this.label6.TabIndex = 59;
            this.label6.Text = "Search";
            // 
            // search_text
            // 
            this.search_text.Location = new System.Drawing.Point(16, 55);
            this.search_text.Margin = new System.Windows.Forms.Padding(4);
            this.search_text.Name = "search_text";
            this.search_text.Size = new System.Drawing.Size(497, 22);
            this.search_text.TabIndex = 58;
            this.search_text.TextChanged += new System.EventHandler(this.search_text_TextChanged);
            // 
            // dataGrid_Categories
            // 
            this.dataGrid_Categories.AllowUserToAddRows = false;
            this.dataGrid_Categories.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGrid_Categories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid_Categories.GridColor = System.Drawing.SystemColors.Control;
            this.dataGrid_Categories.Location = new System.Drawing.Point(18, 131);
            this.dataGrid_Categories.Name = "dataGrid_Categories";
            this.dataGrid_Categories.RowHeadersVisible = false;
            this.dataGrid_Categories.RowHeadersWidth = 51;
            this.dataGrid_Categories.RowTemplate.Height = 24;
            this.dataGrid_Categories.Size = new System.Drawing.Size(493, 478);
            this.dataGrid_Categories.TabIndex = 0;
            this.dataGrid_Categories.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_Categories_CellEndEdit);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 16);
            this.label1.TabIndex = 60;
            this.label1.Text = "Categories";
            // 
            // SQLPowerBIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 710);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGrid_Categories);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.search_text);
            this.Controls.Add(this.btn_save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SQLPowerBIForm";
            this.Text = "SQL PowerBI Export";
            this.Load += new System.EventHandler(this.SQLPowerBIForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid_Categories)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox search_text;
        private System.Windows.Forms.DataGridView dataGrid_Categories;
        private System.Windows.Forms.Label label1;
    }
}