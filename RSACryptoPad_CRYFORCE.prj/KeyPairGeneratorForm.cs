using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RSACryptoPad
{
    public class KeyPairGeneratorForm : Form
    {
        private readonly Container components = null;
        private Button generateKeysButton;
        private PictureBox keyPictureBox;
        private NumericUpDown numericUpDown;

        public KeyPairGeneratorForm()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(KeyPairGeneratorForm));
            generateKeysButton = new Button();
            keyPictureBox = new PictureBox();
            numericUpDown = new NumericUpDown();
            ((ISupportInitialize)(keyPictureBox)).BeginInit();
            ((ISupportInitialize)(numericUpDown)).BeginInit();
            SuspendLayout();
            // 
            // generateKeysButton
            // 
            generateKeysButton.BackColor = SystemColors.Control;
            generateKeysButton.Font = new Font("Georgia", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            generateKeysButton.ForeColor = SystemColors.ControlText;
            generateKeysButton.Location = new Point(83, 45);
            generateKeysButton.Name = "generateKeysButton";
            generateKeysButton.Size = new Size(70, 43);
            generateKeysButton.TabIndex = 0;
            generateKeysButton.Text = "Generate Keys";
            generateKeysButton.UseVisualStyleBackColor = false;
            generateKeysButton.Click += generateKeysButton_Click;
            // 
            // keyPictureBox
            // 
            keyPictureBox.BackColor = Color.Black;
            keyPictureBox.BorderStyle = BorderStyle.Fixed3D;
            keyPictureBox.Image = ((Image)(resources.GetObject("keyPictureBox.Image")));
            keyPictureBox.Location = new Point(12, 12);
            keyPictureBox.Name = "keyPictureBox";
            keyPictureBox.Size = new Size(66, 75);
            keyPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            keyPictureBox.TabIndex = 1;
            keyPictureBox.TabStop = false;
            // 
            // numericUpDown
            // 
            numericUpDown.BackColor = SystemColors.WindowFrame;
            numericUpDown.ForeColor = Color.Lime;
            numericUpDown.Increment = new decimal(new int[]
                                                      {
                                                          8,
                                                          0,
                                                          0,
                                                          0
                                                      });
            numericUpDown.Location = new Point(84, 12);
            numericUpDown.Maximum = new decimal(new int[]
                                                    {
                                                        16384,
                                                        0,
                                                        0,
                                                        0
                                                    });
            numericUpDown.Minimum = new decimal(new int[]
                                                    {
                                                        384,
                                                        0,
                                                        0,
                                                        0
                                                    });
            numericUpDown.Name = "numericUpDown";
            numericUpDown.ReadOnly = true;
            numericUpDown.Size = new Size(68, 26);
            numericUpDown.TabIndex = 0;
            numericUpDown.ThousandsSeparator = true;
            numericUpDown.UpDownAlign = LeftRightAlignment.Left;
            numericUpDown.Value = new decimal(new int[]
                                                  {
                                                      8192,
                                                      0,
                                                      0,
                                                      0
                                                  });
            // 
            // KeyPairGeneratorForm
            // 
            AutoScaleBaseSize = new Size(8, 19);
            BackColor = Color.Black;
            ClientSize = new Size(163, 100);
            Controls.Add(numericUpDown);
            Controls.Add(keyPictureBox);
            Controls.Add(generateKeysButton);
            Font = new Font("Georgia", 12F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            ForeColor = Color.LightGreen;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "KeyPairGeneratorForm";
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Generate Keys";
            Load += KeyPairGeneratorForm_Load;
            ((ISupportInitialize)(keyPictureBox)).EndInit();
            ((ISupportInitialize)(numericUpDown)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void generateKeysButton_Click(object sender, EventArgs e)
        {
            MainForm.SetBitStrength(Convert.ToInt32(numericUpDown.Value));
            DialogResult = DialogResult.OK;
            Dispose(true);
        }

        private void KeyPairGeneratorForm_Load(object sender, EventArgs e)
        {
            MainForm.SetBitStrength(1024);
        }
    }
}