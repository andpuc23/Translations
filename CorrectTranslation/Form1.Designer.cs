﻿
namespace CorrectTranslation
{
    partial class Form1
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
            this.translateBtn = new System.Windows.Forms.Button();
            this.translateRTB = new System.Windows.Forms.RichTextBox();
            this.OrigRTB = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.renderBtn = new System.Windows.Forms.Button();
            this.paragraphsCB = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // translateBtn
            // 
            this.translateBtn.Location = new System.Drawing.Point(598, 496);
            this.translateBtn.Name = "translateBtn";
            this.translateBtn.Size = new System.Drawing.Size(257, 112);
            this.translateBtn.TabIndex = 0;
            this.translateBtn.Text = "Перевести";
            this.translateBtn.UseVisualStyleBackColor = true;
            this.translateBtn.Click += new System.EventHandler(this.translateBtn_Click);
            // 
            // translateRTB
            // 
            this.translateRTB.Location = new System.Drawing.Point(500, 37);
            this.translateRTB.Name = "translateRTB";
            this.translateRTB.Size = new System.Drawing.Size(422, 419);
            this.translateRTB.TabIndex = 1;
            this.translateRTB.Text = "";
            // 
            // OrigRTB
            // 
            this.OrigRTB.Location = new System.Drawing.Point(12, 37);
            this.OrigRTB.Name = "OrigRTB";
            this.OrigRTB.Size = new System.Drawing.Size(369, 176);
            this.OrigRTB.TabIndex = 2;
            this.OrigRTB.Text = "Перетащите файл .tex сюда";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Оригинал";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(569, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Перевод";
            // 
            // renderBtn
            // 
            this.renderBtn.Location = new System.Drawing.Point(368, 496);
            this.renderBtn.Name = "renderBtn";
            this.renderBtn.Size = new System.Drawing.Size(224, 112);
            this.renderBtn.TabIndex = 7;
            this.renderBtn.Text = "Отрисовать ЛаТеХ\n(в разработке)";
            this.renderBtn.UseVisualStyleBackColor = true;
            this.renderBtn.Click += new System.EventHandler(this.renderBtn_Click);
            //
            // paragraphsCB
            //
            this.paragraphsCB.Location = new System.Drawing.Point( 12, 300 );
            this.paragraphsCB.Size = new System.Drawing.Size( 350, 112 );
            this.paragraphsCB.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.paragraphsCB.TabIndex = 7;
            this.paragraphsCB.Name = "paragraphsCB";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 620);
            this.Controls.Add(this.renderBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OrigRTB);
            this.Controls.Add(this.translateRTB);
            this.Controls.Add(this.translateBtn);
            this.Controls.Add( this.paragraphsCB );
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button translateBtn;
        private System.Windows.Forms.RichTextBox translateRTB;
        private System.Windows.Forms.RichTextBox OrigRTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button renderBtn;
        private System.Windows.Forms.ComboBox paragraphsCB;
    }
}

