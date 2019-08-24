namespace CanyonShooter.Editor
{
    partial class CanyonGenerator
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
            this.button1 = new System.Windows.Forms.Button();
            this.labelError = new System.Windows.Forms.Label();
            this.tabGeneratorOptions = new System.Windows.Forms.TabControl();
            this.tabCanyon = new System.Windows.Forms.TabPage();
            this.textBoxTolerance = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxNumberofSegements = new System.Windows.Forms.TextBox();
            this.textBoxReferences = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxSegmentLengthTo = new System.Windows.Forms.TextBox();
            this.textBoxSegmentLengthFrom = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxDifHeight = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxCurviness = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabObjects = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.listEnemies = new System.Windows.Forms.ListBox();
            this.label11 = new System.Windows.Forms.Label();
            this.listQuickItems = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.listPredefinedObjects = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.listBoxObjectsToCreate = new System.Windows.Forms.ListBox();
            this.ObjectProperties = new System.Windows.Forms.PropertyGrid();
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxObjects = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.checkOnlyObjects = new System.Windows.Forms.CheckBox();
            this.tabGeneratorOptions.SuspendLayout();
            this.tabCanyon.SuspendLayout();
            this.tabObjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(841, 462);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Generate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Generate_Click);
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.Location = new System.Drawing.Point(3, 472);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(32, 13);
            this.labelError.TabIndex = 17;
            this.labelError.Text = "Error:";
            // 
            // tabGeneratorOptions
            // 
            this.tabGeneratorOptions.Controls.Add(this.tabCanyon);
            this.tabGeneratorOptions.Controls.Add(this.tabObjects);
            this.tabGeneratorOptions.Location = new System.Drawing.Point(2, 2);
            this.tabGeneratorOptions.Name = "tabGeneratorOptions";
            this.tabGeneratorOptions.SelectedIndex = 0;
            this.tabGeneratorOptions.Size = new System.Drawing.Size(918, 454);
            this.tabGeneratorOptions.TabIndex = 26;
            // 
            // tabCanyon
            // 
            this.tabCanyon.Controls.Add(this.textBoxTolerance);
            this.tabCanyon.Controls.Add(this.label1);
            this.tabCanyon.Controls.Add(this.textBoxNumberofSegements);
            this.tabCanyon.Controls.Add(this.textBoxReferences);
            this.tabCanyon.Controls.Add(this.label8);
            this.tabCanyon.Controls.Add(this.textBoxSegmentLengthTo);
            this.tabCanyon.Controls.Add(this.textBoxSegmentLengthFrom);
            this.tabCanyon.Controls.Add(this.label7);
            this.tabCanyon.Controls.Add(this.textBoxDifHeight);
            this.tabCanyon.Controls.Add(this.label6);
            this.tabCanyon.Controls.Add(this.label5);
            this.tabCanyon.Controls.Add(this.textBoxCurviness);
            this.tabCanyon.Controls.Add(this.label3);
            this.tabCanyon.Location = new System.Drawing.Point(4, 22);
            this.tabCanyon.Name = "tabCanyon";
            this.tabCanyon.Padding = new System.Windows.Forms.Padding(3);
            this.tabCanyon.Size = new System.Drawing.Size(910, 428);
            this.tabCanyon.TabIndex = 0;
            this.tabCanyon.Text = "Canyon Generation";
            this.tabCanyon.UseVisualStyleBackColor = true;
            // 
            // textBoxTolerance
            // 
            this.textBoxTolerance.Location = new System.Drawing.Point(259, 151);
            this.textBoxTolerance.Name = "textBoxTolerance";
            this.textBoxTolerance.Size = new System.Drawing.Size(100, 20);
            this.textBoxTolerance.TabIndex = 36;
            this.textBoxTolerance.Text = "10";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(259, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "PointMoveTolerance(0-100)";
            // 
            // textBoxNumberofSegements
            // 
            this.textBoxNumberofSegements.Location = new System.Drawing.Point(165, 31);
            this.textBoxNumberofSegements.Name = "textBoxNumberofSegements";
            this.textBoxNumberofSegements.Size = new System.Drawing.Size(57, 20);
            this.textBoxNumberofSegements.TabIndex = 34;
            this.textBoxNumberofSegements.Text = "10";
            // 
            // textBoxReferences
            // 
            this.textBoxReferences.Location = new System.Drawing.Point(259, 98);
            this.textBoxReferences.Name = "textBoxReferences";
            this.textBoxReferences.Size = new System.Drawing.Size(168, 20);
            this.textBoxReferences.TabIndex = 33;
            this.textBoxReferences.Text = "0;1;2;";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(256, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(153, 13);
            this.label8.TabIndex = 32;
            this.label8.Text = "ReferenceSegments(a;b;...y;z;)";
            // 
            // textBoxSegmentLengthTo
            // 
            this.textBoxSegmentLengthTo.Location = new System.Drawing.Point(117, 151);
            this.textBoxSegmentLengthTo.Name = "textBoxSegmentLengthTo";
            this.textBoxSegmentLengthTo.Size = new System.Drawing.Size(57, 20);
            this.textBoxSegmentLengthTo.TabIndex = 31;
            this.textBoxSegmentLengthTo.Text = "600";
            // 
            // textBoxSegmentLengthFrom
            // 
            this.textBoxSegmentLengthFrom.Location = new System.Drawing.Point(42, 151);
            this.textBoxSegmentLengthFrom.Name = "textBoxSegmentLengthFrom";
            this.textBoxSegmentLengthFrom.Size = new System.Drawing.Size(57, 20);
            this.textBoxSegmentLengthFrom.TabIndex = 30;
            this.textBoxSegmentLengthFrom.Text = "500";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 135);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(130, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "SegementLenght From To";
            // 
            // textBoxDifHeight
            // 
            this.textBoxDifHeight.Location = new System.Drawing.Point(117, 98);
            this.textBoxDifHeight.Name = "textBoxDifHeight";
            this.textBoxDifHeight.Size = new System.Drawing.Size(57, 20);
            this.textBoxDifHeight.TabIndex = 28;
            this.textBoxDifHeight.Text = "50";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(114, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "difference in height";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "curviness";
            // 
            // textBoxCurviness
            // 
            this.textBoxCurviness.Location = new System.Drawing.Point(42, 98);
            this.textBoxCurviness.Name = "textBoxCurviness";
            this.textBoxCurviness.Size = new System.Drawing.Size(57, 20);
            this.textBoxCurviness.TabIndex = 25;
            this.textBoxCurviness.Text = "300";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Segements to generate:";
            // 
            // tabObjects
            // 
            this.tabObjects.Controls.Add(this.button3);
            this.tabObjects.Controls.Add(this.label12);
            this.tabObjects.Controls.Add(this.listEnemies);
            this.tabObjects.Controls.Add(this.label11);
            this.tabObjects.Controls.Add(this.listQuickItems);
            this.tabObjects.Controls.Add(this.button2);
            this.tabObjects.Controls.Add(this.label10);
            this.tabObjects.Controls.Add(this.listPredefinedObjects);
            this.tabObjects.Controls.Add(this.label9);
            this.tabObjects.Controls.Add(this.listBoxObjectsToCreate);
            this.tabObjects.Controls.Add(this.ObjectProperties);
            this.tabObjects.Controls.Add(this.label4);
            this.tabObjects.Controls.Add(this.listBoxObjects);
            this.tabObjects.Location = new System.Drawing.Point(4, 22);
            this.tabObjects.Name = "tabObjects";
            this.tabObjects.Padding = new System.Windows.Forms.Padding(3);
            this.tabObjects.Size = new System.Drawing.Size(910, 428);
            this.tabObjects.TabIndex = 1;
            this.tabObjects.Text = "Object Management";
            this.tabObjects.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(14, 222);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(132, 13);
            this.label12.TabIndex = 36;
            this.label12.Text = "Avaible Enemy Types:";
            // 
            // listEnemies
            // 
            this.listEnemies.FormattingEnabled = true;
            this.listEnemies.Location = new System.Drawing.Point(17, 238);
            this.listEnemies.Name = "listEnemies";
            this.listEnemies.Size = new System.Drawing.Size(199, 173);
            this.listEnemies.TabIndex = 35;
            this.listEnemies.DoubleClick += new System.EventHandler(this.listEnemies_DoubleClick);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(233, 222);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(157, 13);
            this.label11.TabIndex = 34;
            this.label11.Text = "Avaible Items for Enemies:";
            // 
            // listQuickItems
            // 
            this.listQuickItems.FormattingEnabled = true;
            this.listQuickItems.Location = new System.Drawing.Point(236, 238);
            this.listQuickItems.Name = "listQuickItems";
            this.listQuickItems.Size = new System.Drawing.Size(202, 173);
            this.listQuickItems.TabIndex = 33;
            this.listQuickItems.DoubleClick += new System.EventHandler(this.listQuickItems_DoubleClick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(447, 179);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(190, 36);
            this.button2.TabIndex = 32;
            this.button2.Text = "Save as Predefined";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(444, 222);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 13);
            this.label10.TabIndex = 31;
            this.label10.Text = "Predefined Objects:";
            // 
            // listPredefinedObjects
            // 
            this.listPredefinedObjects.FormattingEnabled = true;
            this.listPredefinedObjects.Location = new System.Drawing.Point(447, 238);
            this.listPredefinedObjects.Name = "listPredefinedObjects";
            this.listPredefinedObjects.Size = new System.Drawing.Size(190, 173);
            this.listPredefinedObjects.TabIndex = 30;
            this.listPredefinedObjects.DoubleClick += new System.EventHandler(this.listPredefinedObjects_DoubleClick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(444, 12);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(144, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "Objects to be generated";
            // 
            // listBoxObjectsToCreate
            // 
            this.listBoxObjectsToCreate.FormattingEnabled = true;
            this.listBoxObjectsToCreate.Location = new System.Drawing.Point(447, 28);
            this.listBoxObjectsToCreate.Name = "listBoxObjectsToCreate";
            this.listBoxObjectsToCreate.Size = new System.Drawing.Size(190, 147);
            this.listBoxObjectsToCreate.TabIndex = 28;
            this.listBoxObjectsToCreate.SelectedValueChanged += new System.EventHandler(this.listBoxObjectsToCreate_SelectedValueChanged);
            this.listBoxObjectsToCreate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxObjectsToCreate_KeyDown);
            // 
            // ObjectProperties
            // 
            this.ObjectProperties.Location = new System.Drawing.Point(643, 0);
            this.ObjectProperties.Name = "ObjectProperties";
            this.ObjectProperties.Size = new System.Drawing.Size(271, 432);
            this.ObjectProperties.TabIndex = 27;
            this.ObjectProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.ObjectProperties_PropertyValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Avaible Object-Types:";
            // 
            // listBoxObjects
            // 
            this.listBoxObjects.FormattingEnabled = true;
            this.listBoxObjects.Location = new System.Drawing.Point(16, 28);
            this.listBoxObjects.Name = "listBoxObjects";
            this.listBoxObjects.Size = new System.Drawing.Size(422, 147);
            this.listBoxObjects.TabIndex = 25;
            this.listBoxObjects.DoubleClick += new System.EventHandler(this.listBoxObjects_DoubleClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(125, 179);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(202, 38);
            this.button3.TabIndex = 37;
            this.button3.Text = "Reload Enemies, Items and Predefined Objects";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // checkOnlyObjects
            // 
            this.checkOnlyObjects.AutoSize = true;
            this.checkOnlyObjects.Location = new System.Drawing.Point(704, 468);
            this.checkOnlyObjects.Name = "checkOnlyObjects";
            this.checkOnlyObjects.Size = new System.Drawing.Size(131, 17);
            this.checkOnlyObjects.TabIndex = 41;
            this.checkOnlyObjects.Text = "Generate only Objects";
            this.checkOnlyObjects.UseVisualStyleBackColor = true;
            // 
            // CanyonGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 497);
            this.Controls.Add(this.checkOnlyObjects);
            this.Controls.Add(this.tabGeneratorOptions);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.button1);
            this.Name = "CanyonGenerator";
            this.Text = "CanyonGenerator";
            this.Load += new System.EventHandler(this.CanyonGenerator_Load);
            this.tabGeneratorOptions.ResumeLayout(false);
            this.tabCanyon.ResumeLayout(false);
            this.tabCanyon.PerformLayout();
            this.tabObjects.ResumeLayout(false);
            this.tabObjects.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.TabControl tabGeneratorOptions;
        private System.Windows.Forms.TabPage tabCanyon;
        private System.Windows.Forms.TextBox textBoxTolerance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNumberofSegements;
        private System.Windows.Forms.TextBox textBoxReferences;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxSegmentLengthTo;
        private System.Windows.Forms.TextBox textBoxSegmentLengthFrom;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxDifHeight;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxCurviness;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabObjects;
        private System.Windows.Forms.ListBox listBoxObjects;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBoxObjectsToCreate;
        private System.Windows.Forms.PropertyGrid ObjectProperties;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListBox listPredefinedObjects;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listQuickItems;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListBox listEnemies;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkOnlyObjects;
    }
}