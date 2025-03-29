namespace Varakin_Oleg_PRI_121_LR_7
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            AnT = new Tao.Platform.Windows.SimpleOpenGlControl();
            RenderTimer = new System.Windows.Forms.Timer(components);
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            button2 = new Button();
            SuspendLayout();
            // 
            // AnT
            // 
            AnT.AccumBits = 0;
            AnT.AutoCheckErrors = false;
            AnT.AutoFinish = false;
            AnT.AutoMakeCurrent = true;
            AnT.AutoSwapBuffers = true;
            AnT.BackColor = Color.Black;
            AnT.ColorBits = 32;
            AnT.DepthBits = 16;
            AnT.Location = new Point(12, 12);
            AnT.Name = "AnT";
            AnT.Size = new Size(871, 578);
            AnT.StencilBits = 0;
            AnT.TabIndex = 0;
            AnT.KeyDown += AnT_KeyDown;
            // 
            // RenderTimer
            // 
            RenderTimer.Tick += RenderTimer_Tick;
            // 
            // button1
            // 
            button1.Location = new Point(905, 12);
            button1.Name = "button1";
            button1.Size = new Size(250, 23);
            button1.TabIndex = 1;
            button1.Text = "Включить черно-белый режим";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(931, 101);
            label1.Name = "label1";
            label1.Size = new Size(198, 15);
            label1.TabIndex = 2;
            label1.Text = "W,A,S,D,Q,E - Управление камерой";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(931, 129);
            label2.Name = "label2";
            label2.Size = new Size(205, 15);
            label2.TabIndex = 3;
            label2.Text = "U,H,J,K - Управление ботинками X,Y";
            // 
            // button2
            // 
            button2.Location = new Point(905, 50);
            button2.Name = "button2";
            button2.Size = new Size(250, 23);
            button2.TabIndex = 4;
            button2.Text = "Включить/Выключить дождь";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1167, 602);
            Controls.Add(button2);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(AnT);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Tao.Platform.Windows.SimpleOpenGlControl AnT;
        private BindingSource bindingSource1;
        private System.Windows.Forms.Timer RenderTimer;
        private Button button1;
        private Label label1;
        private Label label2;
        private Button button2;
    }
}
