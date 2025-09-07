using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowTopMost
{
    public partial class SettingsForm : Form
    {
        public uint Modifiers { get; private set; }
        public uint VirtualKey { get; private set; }
        public bool EnableTransparency { get; private set; }
        public int TransparencyLevel { get; private set; }
        
        // 置顶热键控件
        private CheckBox ctrlCheckBox;
        private CheckBox altCheckBox;
        private CheckBox shiftCheckBox;
        private CheckBox winCheckBox;
        private ComboBox keyComboBox;
        private Label previewLabel;
        
        // 透明度增加热键控件
        private CheckBox transparencyIncreaseCtrlCheckBox;
        private CheckBox transparencyIncreaseAltCheckBox;
        private CheckBox transparencyIncreaseShiftCheckBox;
        private CheckBox transparencyIncreaseWinCheckBox;
        private ComboBox transparencyIncreaseKeyComboBox;
        private Label transparencyIncreasePreviewLabel;
        
        // 透明度减少热键控件
        private CheckBox transparencyDecreaseCtrlCheckBox;
        private CheckBox transparencyDecreaseAltCheckBox;
        private CheckBox transparencyDecreaseShiftCheckBox;
        private CheckBox transparencyDecreaseWinCheckBox;
        private ComboBox transparencyDecreaseKeyComboBox;
        private Label transparencyDecreasePreviewLabel;
        
        // 透明度设置控件
        private CheckBox transparencyCheckBox;
        private TrackBar transparencyTrackBar;
        private Label transparencyValueLabel;
        
        // 通用控件
        private TabControl tabControl;
        private Button okButton;
        private Button cancelButton;
        
        // 透明度热键属性
        public uint TransparencyIncreaseModifiers { get; private set; }
        public uint TransparencyIncreaseVirtualKey { get; private set; }
        public uint TransparencyDecreaseModifiers { get; private set; }
        public uint TransparencyDecreaseVirtualKey { get; private set; }

        public SettingsForm(uint currentModifiers, uint currentVirtualKey, bool enableTransparency = false, int transparencyLevel = 200, uint transparencyIncreaseModifiers = 0, uint transparencyIncreaseVirtualKey = 0, uint transparencyDecreaseModifiers = 0, uint transparencyDecreaseVirtualKey = 0)
        {
            Modifiers = currentModifiers;
            VirtualKey = currentVirtualKey;
            EnableTransparency = enableTransparency;
            TransparencyLevel = transparencyLevel;
            TransparencyIncreaseModifiers = transparencyIncreaseModifiers;
            TransparencyIncreaseVirtualKey = transparencyIncreaseVirtualKey;
            TransparencyDecreaseModifiers = transparencyDecreaseModifiers;
            TransparencyDecreaseVirtualKey = transparencyDecreaseVirtualKey;
            
            InitializeComponent();
            LoadCurrentSettings();
            UpdatePreview();
        }

        private void InitializeComponent()
        {
            this.Text = "设置";
            this.Size = new Size(420, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 创建TabControl
            tabControl = new TabControl()
            {
                Location = new Point(10, 10),
                Size = new Size(380, 410),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            this.Controls.Add(tabControl);

            // 创建置顶热键选项卡
            var topMostTab = new TabPage("置顶热键");
            tabControl.TabPages.Add(topMostTab);
            CreateTopMostHotkeyControls(topMostTab);

            // 创建透明度选项卡
            var transparencyTab = new TabPage("透明度");
            tabControl.TabPages.Add(transparencyTab);
            CreateTransparencyControls(transparencyTab);

            // 按钮
            okButton = new Button()
            {
                Text = "确定",
                Location = new Point(210, 430),
                Size = new Size(75, 25),
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;
            this.Controls.Add(okButton);

            cancelButton = new Button()
            {
                Text = "取消",
                Location = new Point(295, 430),
                Size = new Size(75, 25),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void CreateTopMostHotkeyControls(TabPage parent)
        {
            // 修饰键标签
            var modifiersLabel = new Label()
            {
                Text = "修饰键：",
                Location = new Point(20, 20),
                Size = new Size(60, 20)
            };
            parent.Controls.Add(modifiersLabel);

            // 修饰键复选框
            ctrlCheckBox = new CheckBox()
            {
                Text = "Ctrl",
                Location = new Point(90, 20),
                Size = new Size(60, 20)
            };
            ctrlCheckBox.CheckedChanged += ModifierCheckBox_CheckedChanged;
            parent.Controls.Add(ctrlCheckBox);

            altCheckBox = new CheckBox()
            {
                Text = "Alt",
                Location = new Point(160, 20),
                Size = new Size(50, 20)
            };
            altCheckBox.CheckedChanged += ModifierCheckBox_CheckedChanged;
            parent.Controls.Add(altCheckBox);

            shiftCheckBox = new CheckBox()
            {
                Text = "Shift",
                Location = new Point(220, 20),
                Size = new Size(60, 20)
            };
            shiftCheckBox.CheckedChanged += ModifierCheckBox_CheckedChanged;
            parent.Controls.Add(shiftCheckBox);

            winCheckBox = new CheckBox()
            {
                Text = "Win",
                Location = new Point(290, 20),
                Size = new Size(50, 20)
            };
            winCheckBox.CheckedChanged += ModifierCheckBox_CheckedChanged;
            parent.Controls.Add(winCheckBox);

            // 按键标签
            var keyLabel = new Label()
            {
                Text = "按键：",
                Location = new Point(20, 60),
                Size = new Size(60, 20)
            };
            parent.Controls.Add(keyLabel);

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
            parent.Controls.Add(keyComboBox);

            // 预览标签
            var previewTitleLabel = new Label()
            {
                Text = "当前热键：",
                Location = new Point(20, 100),
                Size = new Size(80, 20)
            };
            parent.Controls.Add(previewTitleLabel);

            previewLabel = new Label()
            {
                Location = new Point(110, 100),
                Size = new Size(200, 20),
                Font = new Font("Microsoft YaHei", 9, FontStyle.Bold),
                ForeColor = Color.Blue
            };
            parent.Controls.Add(previewLabel);

            // 说明标签
            var infoLabel = new Label()
            {
                Text = "注意：必须至少选择一个修饰键，避免与系统热键冲突。",
                Location = new Point(20, 130),
                Size = new Size(300, 40),
                ForeColor = Color.Gray
            };
            parent.Controls.Add(infoLabel);
        }

        private void CreateTransparencyControls(TabPage parent)
        {
            // 透明度增加热键设置分组
            var increaseGroupBox = new GroupBox()
            {
                Text = "透明度增加热键",
                Location = new Point(20, 20),
                Size = new Size(360, 120)
            };
            parent.Controls.Add(increaseGroupBox);

            CreateTransparencyHotkeyControls(increaseGroupBox, true, 0);

            // 透明度减少热键设置分组
            var decreaseGroupBox = new GroupBox()
            {
                Text = "透明度减少热键",
                Location = new Point(20, 150),
                Size = new Size(360, 120)
            };
            parent.Controls.Add(decreaseGroupBox);

            CreateTransparencyHotkeyControls(decreaseGroupBox, false, 0);

            // 透明度设置分组
            var transparencyGroupBox = new GroupBox()
            {
                Text = "透明度设置",
                Location = new Point(20, 280),
                Size = new Size(360, 100)
            };
            parent.Controls.Add(transparencyGroupBox);

            // 启用透明度复选框
            transparencyCheckBox = new CheckBox()
            {
                Text = "启用窗口透明度",
                Location = new Point(15, 25),
                Size = new Size(120, 20)
            };
            transparencyCheckBox.CheckedChanged += TransparencyCheckBox_CheckedChanged;
            transparencyGroupBox.Controls.Add(transparencyCheckBox);

            // 透明度滑块
            transparencyTrackBar = new TrackBar()
            {
                Location = new Point(15, 50),
                Size = new Size(200, 45),
                Minimum = 50,  // 最小50，避免完全透明
                Maximum = 255, // 最大255，完全不透明
                TickFrequency = 25,
                Value = 200
            };
            transparencyTrackBar.ValueChanged += TransparencyTrackBar_ValueChanged;
            transparencyGroupBox.Controls.Add(transparencyTrackBar);

            // 透明度数值标签
            transparencyValueLabel = new Label()
            {
                Location = new Point(225, 60),
                Size = new Size(80, 20),
                Text = "78%",
                TextAlign = ContentAlignment.MiddleLeft
            };
            transparencyGroupBox.Controls.Add(transparencyValueLabel);

        }

        private void CreateTransparencyHotkeyControls(GroupBox parent, bool isIncrease, int yOffset)
        {
            // 修饰键标签
            var modifiersLabel = new Label()
            {
                Text = "修饰键：",
                Location = new Point(15, 25 + yOffset),
                Size = new Size(60, 20)
            };
            parent.Controls.Add(modifiersLabel);

            // 修饰键复选框
            CheckBox ctrlCheckBox, altCheckBox, shiftCheckBox, winCheckBox;
            ComboBox keyComboBox;
            Label previewLabel;

            if (isIncrease)
            {
                CreateModifierCheckBoxes(out transparencyIncreaseCtrlCheckBox, out transparencyIncreaseAltCheckBox, 
                                        out transparencyIncreaseShiftCheckBox, out transparencyIncreaseWinCheckBox, 
                                        yOffset, TransparencyIncreaseModifierCheckBox_CheckedChanged);
                
                ctrlCheckBox = transparencyIncreaseCtrlCheckBox;
                altCheckBox = transparencyIncreaseAltCheckBox;
                shiftCheckBox = transparencyIncreaseShiftCheckBox;
                winCheckBox = transparencyIncreaseWinCheckBox;
            }
            else
            {
                CreateModifierCheckBoxes(out transparencyDecreaseCtrlCheckBox, out transparencyDecreaseAltCheckBox, 
                                        out transparencyDecreaseShiftCheckBox, out transparencyDecreaseWinCheckBox, 
                                        yOffset, TransparencyDecreaseModifierCheckBox_CheckedChanged);
                
                ctrlCheckBox = transparencyDecreaseCtrlCheckBox;
                altCheckBox = transparencyDecreaseAltCheckBox;
                shiftCheckBox = transparencyDecreaseShiftCheckBox;
                winCheckBox = transparencyDecreaseWinCheckBox;
            }

            parent.Controls.Add(ctrlCheckBox);
            parent.Controls.Add(altCheckBox);
            parent.Controls.Add(shiftCheckBox);
            parent.Controls.Add(winCheckBox);

            // 按键标签
            var keyLabel = new Label()
            {
                Text = "按键：",
                Location = new Point(15, 55 + yOffset),
                Size = new Size(60, 20)
            };
            parent.Controls.Add(keyLabel);

            // 按键下拉框
            if (isIncrease)
            {
                transparencyIncreaseKeyComboBox = new ComboBox()
                {
                    Location = new Point(85, 53 + yOffset),
                    Size = new Size(100, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                keyComboBox = transparencyIncreaseKeyComboBox;
                keyComboBox.SelectedIndexChanged += TransparencyIncreaseKeyComboBox_SelectedIndexChanged;
            }
            else
            {
                transparencyDecreaseKeyComboBox = new ComboBox()
                {
                    Location = new Point(85, 53 + yOffset),
                    Size = new Size(100, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                keyComboBox = transparencyDecreaseKeyComboBox;
                keyComboBox.SelectedIndexChanged += TransparencyDecreaseKeyComboBox_SelectedIndexChanged;
            }

            string[] commonKeys = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                                  "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                  "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12",
                                  "Space", "Enter", "Escape", "Tab", "Insert", "Delete", "Home", "End",
                                  "PageUp", "PageDown", "Up", "Down", "Left", "Right" };
            
            keyComboBox.Items.AddRange(commonKeys);
            parent.Controls.Add(keyComboBox);

            // 预览标签
            var previewTitleLabel = new Label()
            {
                Text = "当前热键：",
                Location = new Point(200, 55 + yOffset),
                Size = new Size(80, 20)
            };
            parent.Controls.Add(previewTitleLabel);

            if (isIncrease)
            {
                transparencyIncreasePreviewLabel = new Label()
                {
                    Location = new Point(280, 55 + yOffset),
                    Size = new Size(70, 20),
                    Font = new Font("Microsoft YaHei", 8, FontStyle.Bold),
                    ForeColor = Color.Blue
                };
                previewLabel = transparencyIncreasePreviewLabel;
            }
            else
            {
                transparencyDecreasePreviewLabel = new Label()
                {
                    Location = new Point(280, 55 + yOffset),
                    Size = new Size(70, 20),
                    Font = new Font("Microsoft YaHei", 8, FontStyle.Bold),
                    ForeColor = Color.Blue
                };
                previewLabel = transparencyDecreasePreviewLabel;
            }
            parent.Controls.Add(previewLabel);

        }

        private void LoadCurrentSettings()
        {
            // 加载置顶热键设置
            ctrlCheckBox.Checked = (Modifiers & WindowsAPI.MOD_CONTROL) != 0;
            altCheckBox.Checked = (Modifiers & WindowsAPI.MOD_ALT) != 0;
            shiftCheckBox.Checked = (Modifiers & WindowsAPI.MOD_SHIFT) != 0;
            winCheckBox.Checked = (Modifiers & WindowsAPI.MOD_WIN) != 0;

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

            // 加载透明度增加热键设置
            transparencyIncreaseCtrlCheckBox.Checked = (TransparencyIncreaseModifiers & WindowsAPI.MOD_CONTROL) != 0;
            transparencyIncreaseAltCheckBox.Checked = (TransparencyIncreaseModifiers & WindowsAPI.MOD_ALT) != 0;
            transparencyIncreaseShiftCheckBox.Checked = (TransparencyIncreaseModifiers & WindowsAPI.MOD_SHIFT) != 0;
            transparencyIncreaseWinCheckBox.Checked = (TransparencyIncreaseModifiers & WindowsAPI.MOD_WIN) != 0;

            if (TransparencyIncreaseVirtualKey != 0)
            {
                string transparencyIncreaseKeyName = ((Keys)TransparencyIncreaseVirtualKey).ToString();
                int transparencyIncreaseIndex = transparencyIncreaseKeyComboBox.FindStringExact(transparencyIncreaseKeyName);
                if (transparencyIncreaseIndex >= 0)
                {
                    transparencyIncreaseKeyComboBox.SelectedIndex = transparencyIncreaseIndex;
                }
                else
                {
                    transparencyIncreaseKeyComboBox.Text = transparencyIncreaseKeyName;
                }
            }

            // 加载透明度减少热键设置
            transparencyDecreaseCtrlCheckBox.Checked = (TransparencyDecreaseModifiers & WindowsAPI.MOD_CONTROL) != 0;
            transparencyDecreaseAltCheckBox.Checked = (TransparencyDecreaseModifiers & WindowsAPI.MOD_ALT) != 0;
            transparencyDecreaseShiftCheckBox.Checked = (TransparencyDecreaseModifiers & WindowsAPI.MOD_SHIFT) != 0;
            transparencyDecreaseWinCheckBox.Checked = (TransparencyDecreaseModifiers & WindowsAPI.MOD_WIN) != 0;

            if (TransparencyDecreaseVirtualKey != 0)
            {
                string transparencyDecreaseKeyName = ((Keys)TransparencyDecreaseVirtualKey).ToString();
                int transparencyDecreaseIndex = transparencyDecreaseKeyComboBox.FindStringExact(transparencyDecreaseKeyName);
                if (transparencyDecreaseIndex >= 0)
                {
                    transparencyDecreaseKeyComboBox.SelectedIndex = transparencyDecreaseIndex;
                }
                else
                {
                    transparencyDecreaseKeyComboBox.Text = transparencyDecreaseKeyName;
                }
            }

            // 加载透明度设置
            transparencyCheckBox.Checked = EnableTransparency;
            transparencyTrackBar.Value = Math.Max(50, Math.Min(255, TransparencyLevel));
            transparencyTrackBar.Enabled = EnableTransparency;
            UpdateTransparencyLabel();
            UpdateTransparencyIncreasePreview();
            UpdateTransparencyDecreasePreview();
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

        private void TransparencyIncreaseModifierCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTransparencyIncreasePreview();
            ValidateSettings();
        }

        private void TransparencyIncreaseKeyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTransparencyIncreasePreview();
            ValidateSettings();
        }

        private void TransparencyDecreaseModifierCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTransparencyDecreasePreview();
            ValidateSettings();
        }

        private void TransparencyDecreaseKeyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTransparencyDecreasePreview();
            ValidateSettings();
        }

        private void UpdatePreview()
        {
            UpdateHotkeyPreview(ctrlCheckBox, altCheckBox, shiftCheckBox, winCheckBox, keyComboBox, previewLabel);
        }

        private void UpdateTransparencyIncreasePreview()
        {
            UpdateHotkeyPreview(transparencyIncreaseCtrlCheckBox, transparencyIncreaseAltCheckBox, 
                              transparencyIncreaseShiftCheckBox, transparencyIncreaseWinCheckBox, 
                              transparencyIncreaseKeyComboBox, transparencyIncreasePreviewLabel);
        }

        private void UpdateTransparencyDecreasePreview()
        {
            UpdateHotkeyPreview(transparencyDecreaseCtrlCheckBox, transparencyDecreaseAltCheckBox, 
                              transparencyDecreaseShiftCheckBox, transparencyDecreaseWinCheckBox, 
                              transparencyDecreaseKeyComboBox, transparencyDecreasePreviewLabel);
        }

        private void UpdateHotkeyPreview(CheckBox ctrlCheckBox, CheckBox altCheckBox, CheckBox shiftCheckBox, 
                                       CheckBox winCheckBox, ComboBox keyComboBox, Label previewLabel)
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

        private void CreateModifierCheckBoxes(out CheckBox ctrlCheckBox, out CheckBox altCheckBox, 
                                             out CheckBox shiftCheckBox, out CheckBox winCheckBox, 
                                             int yOffset, EventHandler checkedChangedHandler)
        {
            ctrlCheckBox = new CheckBox() { Text = "Ctrl", Location = new Point(85, 25 + yOffset), Size = new Size(60, 20) };
            altCheckBox = new CheckBox() { Text = "Alt", Location = new Point(155, 25 + yOffset), Size = new Size(50, 20) };
            shiftCheckBox = new CheckBox() { Text = "Shift", Location = new Point(215, 25 + yOffset), Size = new Size(60, 20) };
            winCheckBox = new CheckBox() { Text = "Win", Location = new Point(285, 25 + yOffset), Size = new Size(50, 20) };
            
            ctrlCheckBox.CheckedChanged += checkedChangedHandler;
            altCheckBox.CheckedChanged += checkedChangedHandler;
            shiftCheckBox.CheckedChanged += checkedChangedHandler;
            winCheckBox.CheckedChanged += checkedChangedHandler;
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

        private void TransparencyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            transparencyTrackBar.Enabled = transparencyCheckBox.Checked;
            EnableTransparency = transparencyCheckBox.Checked;
        }

        private void TransparencyTrackBar_ValueChanged(object sender, EventArgs e)
        {
            TransparencyLevel = transparencyTrackBar.Value;
            UpdateTransparencyLabel();
        }

        private void UpdateTransparencyLabel()
        {
            int percentage = (int)Math.Round((double)transparencyTrackBar.Value / 255 * 100);
            transparencyValueLabel.Text = $"{percentage}%";
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            // 保存置顶热键修饰键
            uint modifiers = 0;
            if (ctrlCheckBox.Checked) modifiers |= WindowsAPI.MOD_CONTROL;
            if (altCheckBox.Checked) modifiers |= WindowsAPI.MOD_ALT;
            if (shiftCheckBox.Checked) modifiers |= WindowsAPI.MOD_SHIFT;
            if (winCheckBox.Checked) modifiers |= WindowsAPI.MOD_WIN;

            // 获取置顶热键按键值
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

            // 保存透明度增加热键修饰键
            uint transparencyIncreaseModifiers = 0;
            if (transparencyIncreaseCtrlCheckBox.Checked) transparencyIncreaseModifiers |= WindowsAPI.MOD_CONTROL;
            if (transparencyIncreaseAltCheckBox.Checked) transparencyIncreaseModifiers |= WindowsAPI.MOD_ALT;
            if (transparencyIncreaseShiftCheckBox.Checked) transparencyIncreaseModifiers |= WindowsAPI.MOD_SHIFT;
            if (transparencyIncreaseWinCheckBox.Checked) transparencyIncreaseModifiers |= WindowsAPI.MOD_WIN;

            // 保存透明度增加热键按键
            if (transparencyIncreaseKeyComboBox.SelectedItem != null)
            {
                string selectedKey = transparencyIncreaseKeyComboBox.SelectedItem.ToString();
                if (Enum.TryParse(selectedKey, out Keys transparencyIncreaseKey))
                {
                    TransparencyIncreaseModifiers = transparencyIncreaseModifiers;
                    TransparencyIncreaseVirtualKey = (uint)transparencyIncreaseKey;
                }
            }

            // 保存透明度减少热键修饰键
            uint transparencyDecreaseModifiers = 0;
            if (transparencyDecreaseCtrlCheckBox.Checked) transparencyDecreaseModifiers |= WindowsAPI.MOD_CONTROL;
            if (transparencyDecreaseAltCheckBox.Checked) transparencyDecreaseModifiers |= WindowsAPI.MOD_ALT;
            if (transparencyDecreaseShiftCheckBox.Checked) transparencyDecreaseModifiers |= WindowsAPI.MOD_SHIFT;
            if (transparencyDecreaseWinCheckBox.Checked) transparencyDecreaseModifiers |= WindowsAPI.MOD_WIN;

            // 保存透明度减少热键按键
            if (transparencyDecreaseKeyComboBox.SelectedItem != null)
            {
                string selectedKey = transparencyDecreaseKeyComboBox.SelectedItem.ToString();
                if (Enum.TryParse(selectedKey, out Keys transparencyDecreaseKey))
                {
                    TransparencyDecreaseModifiers = transparencyDecreaseModifiers;
                    TransparencyDecreaseVirtualKey = (uint)transparencyDecreaseKey;
                }
            }

            // 保存透明度设置
            EnableTransparency = transparencyCheckBox.Checked;
            TransparencyLevel = transparencyTrackBar.Value;
        }
    }
}