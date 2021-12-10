using DALSA.SaperaLT.SapClassBasic;
using DALSA.SaperaLT.SapClassGui;

namespace adqGenieNano
{
    partial class principalForm
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_StartFrameAdq = new System.Windows.Forms.Button();
            this.mainPictureBox = new System.Windows.Forms.PictureBox();
            this.button_StopFrameAdq = new System.Windows.Forms.Button();
            this.numericUpDown_Treshold = new System.Windows.Forms.NumericUpDown();
            this.label_treshold = new System.Windows.Forms.Label();
            this.pictureBoxImageTest = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Treshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImageTest)).BeginInit();
            this.SuspendLayout();
            // 
            // button_StartFrameAdq
            // 
            this.button_StartFrameAdq.Location = new System.Drawing.Point(29, 48);
            this.button_StartFrameAdq.Margin = new System.Windows.Forms.Padding(4);
            this.button_StartFrameAdq.Name = "button_StartFrameAdq";
            this.button_StartFrameAdq.Size = new System.Drawing.Size(175, 28);
            this.button_StartFrameAdq.TabIndex = 1;
            this.button_StartFrameAdq.Text = "Start Frame Adquisition";
            this.button_StartFrameAdq.UseVisualStyleBackColor = true;
            this.button_StartFrameAdq.Click += new System.EventHandler(this.button_SingleFrame_Click);
            // 
            // mainPictureBox
            // 
            this.mainPictureBox.Location = new System.Drawing.Point(265, 48);
            this.mainPictureBox.Name = "mainPictureBox";
            this.mainPictureBox.Size = new System.Drawing.Size(563, 405);
            this.mainPictureBox.TabIndex = 2;
            this.mainPictureBox.TabStop = false;
            // 
            // button_StopFrameAdq
            // 
            this.button_StopFrameAdq.Location = new System.Drawing.Point(29, 123);
            this.button_StopFrameAdq.Name = "button_StopFrameAdq";
            this.button_StopFrameAdq.Size = new System.Drawing.Size(175, 23);
            this.button_StopFrameAdq.TabIndex = 3;
            this.button_StopFrameAdq.Text = "Stop Frame Adquisition";
            this.button_StopFrameAdq.UseVisualStyleBackColor = true;
            this.button_StopFrameAdq.Click += new System.EventHandler(this.button_StopFrameAdq_Click);
            // 
            // numericUpDown_Treshold
            // 
            this.numericUpDown_Treshold.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown_Treshold.Location = new System.Drawing.Point(29, 258);
            this.numericUpDown_Treshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown_Treshold.Name = "numericUpDown_Treshold";
            this.numericUpDown_Treshold.Size = new System.Drawing.Size(120, 22);
            this.numericUpDown_Treshold.TabIndex = 4;
            this.numericUpDown_Treshold.ValueChanged += new System.EventHandler(this.numericUpDown_Treshold_ValueChanged);
            // 
            // label_treshold
            // 
            this.label_treshold.AutoSize = true;
            this.label_treshold.Location = new System.Drawing.Point(29, 235);
            this.label_treshold.Name = "label_treshold";
            this.label_treshold.Size = new System.Drawing.Size(64, 17);
            this.label_treshold.TabIndex = 5;
            this.label_treshold.Text = "Treshold";
            // 
            // pictureBoxImageTest
            // 
            this.pictureBoxImageTest.Location = new System.Drawing.Point(868, 48);
            this.pictureBoxImageTest.Name = "pictureBoxImageTest";
            this.pictureBoxImageTest.Size = new System.Drawing.Size(563, 405);
            this.pictureBoxImageTest.TabIndex = 6;
            this.pictureBoxImageTest.TabStop = false;
            // 
            // principalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1460, 703);
            this.Controls.Add(this.pictureBoxImageTest);
            this.Controls.Add(this.label_treshold);
            this.Controls.Add(this.numericUpDown_Treshold);
            this.Controls.Add(this.button_StopFrameAdq);
            this.Controls.Add(this.mainPictureBox);
            this.Controls.Add(this.button_StartFrameAdq);
            this.Name = "principalForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Treshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImageTest)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_StartFrameAdq;
        private System.Windows.Forms.PictureBox mainPictureBox;
        private System.Windows.Forms.Button button_StopFrameAdq;
        private System.Windows.Forms.NumericUpDown numericUpDown_Treshold;
        private System.Windows.Forms.Label label_treshold;




        private SapAcqDevice m_AcqDevice;
        //private SapBuffer m_Buffers;
        static SapBuffer m_Buffers;
        private SapAcqDeviceToBuf m_Xfer;
        private SapView m_View;
        private SapLocation m_ServerLocation;
        private string m_ConfigFileName;

        //System menu
        //private SystemMenu m_SystemMenu;
        //index for "about this.." item im system menu
        private const int m_AboutID = 0x100;
        private System.Windows.Forms.PictureBox pictureBoxImageTest;
    }
}

