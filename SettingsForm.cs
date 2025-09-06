using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowTopMost
{
    public partial class SettingsForm : Form
    {
        public uint Modifiers { get; private set; }
        public uint VirtualKey { get; private set; }
        
        private CheckBox ctrlCheckBox;
        private CheckBox altCheckBox;
        private CheckBox shiftCheckBox;
        private CheckBox winCheckBox;
        private ComboBox keyComboBox;
        private Button okButton;
        private Button cancelButton;
        private Label previewLabel;

        public SettingsForm(uint currentModifiers, uint currentVirtualKey)
        {
            Modifiers = currentModifiers;
            VirtualKey = currentVirtualKey;
            
            InitializeComponent();
            LoadCurrentSettings();
            UpdatePreview();
        }

        private void InitializeComponent()
        {
            this.Text = "热键设置";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 修饰键标签
            var modifiersLabel = new Label()
            {
                Text = "修饰键：",
                Location = new Point(20, 20),
                Size = new Size(60, 20)
            };
            this.Controls.Add(modifiersLabel);

            // 修饰键复选框
            ctrlCheckBox = new CheckBox()
            {
                Text = "Ctrl",
                Location = new Point(90, 20),
                Size = new Size(60, 20)
            };
            ctrlCheckBox.CheckedChanged += ModifierCheckBox_CheckedChanged;
            this.Controls.Add(ctrlCheckBox);

            altCheckBox = new CheckBox()
            {
                Text = "Alt",
                Location = new Point(160, 20),
                Size = new Size(50, 20)
            };
            altCheckBox.CheckedChanged += ModifierCheckBox_CheckedChanged;
            this.Controls.Add(altCheckBox);

            shiftCheckBox = new CheckBox()
            {
                Text = "Shift",
                Location = new Point(220, 20),
                Size = new Size(60, 20)
            };
            shiftCheckBox.CheckedChanged += ModifierCheckBox_CheckedChanged;
            this.Controls.Add(shiftCheckBox);

            winCheckBox = new CheckBox()
            {
                Text = "Win",
                Location = new Point(290, 20),
                Size = new Size(50, 20)
            };
            winCheckBox.CheckedChanged += ModifierCheckBox_CheckedChanged;
            this.Controls.Add(winCheckBox);

            // 按键标签
            var keyLabel = new Label()
            {
                Text = "按键：",
                Location = new Point(20, 60),
                Size = new Size(60, 20)
            };
            this.Controls.Add(keyLabel);

            // 按键下拉框
            keyComboBox = new ComboBox()
            {
                Location = new Point(90, 58),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            // 添加常用按键
            string[] commonKeys = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                                  "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                  "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12",
                                  "Space", "Enter", "Escape", "Tab", "Insert", "Delete", "Home", "End",
                                  "PageUp", "PageDown", "Up", "Down", "Left", "Right" };
            
            keyComboBox.Items.AddRange(commonKeys);
            keyComboBox.SelectedIndexChanged += KeyComboBox_SelectedIndexChanged;
            this.Controls.Add(keyComboBox);

            // 预览标签
            var previewTitleLabel = new Label()
            {
                Text = "当前热键：",
                Location = new Point(20, 100),
                Size = new Size(80, 20)
            };
            this.Controls.Add(previewTitleLabel);

            previewLabel = new Label()
            {
                Location = new Point(110, 100),
                Size = new Size(200, 20),
                Font = new Font("Microsoft YaHei", 9, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            this.Controls.Add(previewLabel);

            // 说明标签
            var infoLabel = new Label()
            {
                Text = "注意：必须至少选择一个修饰键，避免与系统热键冲突。",
                Location = new Point(20, 130),
                Size = new Size(300, 40),
                ForeColor = Color.Gray
            };
            this.Controls.Add(infoLabel);

            // 按钮
            okButton = new Button()
            {
                Text = "确定",
                Location = new Point(170, 180),
                Size = new Size(75, 25),
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;
            this.Controls.Add(okButton);

            cancelButton = new Button()
            {
                Text = "取消",
                Location = new Point(255, 180),
                Size = new Size(75, 25),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void LoadCurrentSettings()
        {
            // 加载当前修饰键设置
            ctrlCheckBox.Checked = (Modifiers & WindowsAPI.MOD_CONTROL) != 0;
            altCheckBox.Checked = (Modifiers & WindowsAPI.MOD_ALT) != 0;
            shiftCheckBox.Checked = (Modifiers & WindowsAPI.MOD_SHIFT) != 0;
            winCheckBox.Checked = (Modifiers & WindowsAPI.MOD_WIN) != 0;

            // 加载当前按键设置
            string keyName = ((Keys)VirtualKey).ToString();
            int index = keyComboBox.FindStringExact(keyName);
            if (index >= 0)
            {
                keyComboBox.SelectedIndex = index;
            }
            else
            {
                keyComboBox.Text = keyName;
            }
        }

        private void ModifierCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePreview();
            ValidateSettings();
        }

        private void KeyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
            ValidateSettings();
        }

        private void UpdatePreview()
        {
            string preview = "";
            
            if (ctrlCheckBox.Checked) preview += "Ctrl+";
            if (altCheckBox.Checked) preview += "Alt+";
            if (shiftCheckBox.Checked) preview += "Shift+";
            if (winCheckBox.Checked) preview += "Win+";
            
            if (keyComboBox.SelectedItem != null)
            {
                preview += keyComboBox.SelectedItem.ToString();
            }
            else if (!string.IsNullOrEmpty(keyComboBox.Text))
            {
                preview += keyComboBox.Text;
            }

            previewLabel.Text = string.IsNullOrEmpty(preview) ? "(未设置)" : preview;
        }

        private void ValidateSettings()
        {
            // 检查是否至少选择了一个修饰键
            bool hasModifier = ctrlCheckBox.Checked || altCheckBox.Checked || 
                              shiftCheckBox.Checked || winCheckBox.Checked;
            
            // 检查是否选择了按键
            bool hasKey = keyComboBox.SelectedItem != null || !string.IsNullOrEmpty(keyComboBox.Text);
            
            okButton.Enabled = hasModifier && hasKey;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            // 构建修饰键值
            uint modifiers = 0;
            if (ctrlCheckBox.Checked) modifiers |= WindowsAPI.MOD_CONTROL;
            if (altCheckBox.Checked) modifiers |= WindowsAPI.MOD_ALT;
            if (shiftCheckBox.Checked) modifiers |= WindowsAPI.MOD_SHIFT;
            if (winCheckBox.Checked) modifiers |= WindowsAPI.MOD_WIN;

            // 获取按键值
            string keyText = keyComboBox.SelectedItem?.ToString() ?? keyComboBox.Text;
            if (Enum.TryParse<Keys>(keyText, out Keys key))
            {
                Modifiers = modifiers;
                VirtualKey = (uint)key;
            }
            else
            {
                MessageBox.Show("无效的按键选择！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
        }
    }
}