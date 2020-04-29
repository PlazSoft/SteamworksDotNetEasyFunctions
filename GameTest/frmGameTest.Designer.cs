namespace GameTest
{
    partial class frmGameTest
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
            this.components = new System.ComponentModel.Container();
            this.btnUploadWorkshop = new System.Windows.Forms.Button();
            this.txtDebug = new System.Windows.Forms.TextBox();
            this.btnSendStats = new System.Windows.Forms.Button();
            this.btnCheckOwnership = new System.Windows.Forms.Button();
            this.btnVerifyAntiCheat = new System.Windows.Forms.Button();
            this.btnQueryWorkshop = new System.Windows.Forms.Button();
            this.btnUploadProgress = new System.Windows.Forms.Button();
            this.btnGetSubscribed = new System.Windows.Forms.Button();
            this.CheckPublishIDTimer = new System.Windows.Forms.Timer(this.components);
            this.CheckUploadTimer = new System.Windows.Forms.Timer(this.components);
            this.UploadProgress = new System.Windows.Forms.ProgressBar();
            this.CheckCallBacks = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnUploadWorkshop
            // 
            this.btnUploadWorkshop.Location = new System.Drawing.Point(318, 4);
            this.btnUploadWorkshop.Name = "btnUploadWorkshop";
            this.btnUploadWorkshop.Size = new System.Drawing.Size(101, 34);
            this.btnUploadWorkshop.TabIndex = 0;
            this.btnUploadWorkshop.Text = "Upload Workshop";
            this.btnUploadWorkshop.UseVisualStyleBackColor = true;
            this.btnUploadWorkshop.Click += new System.EventHandler(this.btnUploadWorkshop_Click);
            // 
            // txtDebug
            // 
            this.txtDebug.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtDebug.Location = new System.Drawing.Point(0, 168);
            this.txtDebug.Multiline = true;
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.Size = new System.Drawing.Size(800, 282);
            this.txtDebug.TabIndex = 1;
            // 
            // btnSendStats
            // 
            this.btnSendStats.Location = new System.Drawing.Point(12, 4);
            this.btnSendStats.Name = "btnSendStats";
            this.btnSendStats.Size = new System.Drawing.Size(96, 51);
            this.btnSendStats.TabIndex = 2;
            this.btnSendStats.Text = "Send Stats";
            this.btnSendStats.UseVisualStyleBackColor = true;
            this.btnSendStats.Click += new System.EventHandler(this.btnSendStats_Click);
            // 
            // btnCheckOwnership
            // 
            this.btnCheckOwnership.Location = new System.Drawing.Point(114, 4);
            this.btnCheckOwnership.Name = "btnCheckOwnership";
            this.btnCheckOwnership.Size = new System.Drawing.Size(96, 51);
            this.btnCheckOwnership.TabIndex = 3;
            this.btnCheckOwnership.Text = "Check DLC Ownership";
            this.btnCheckOwnership.UseVisualStyleBackColor = true;
            this.btnCheckOwnership.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnVerifyAntiCheat
            // 
            this.btnVerifyAntiCheat.Location = new System.Drawing.Point(216, 4);
            this.btnVerifyAntiCheat.Name = "btnVerifyAntiCheat";
            this.btnVerifyAntiCheat.Size = new System.Drawing.Size(96, 51);
            this.btnVerifyAntiCheat.TabIndex = 4;
            this.btnVerifyAntiCheat.Text = "Check AntiCheat Ticket";
            this.btnVerifyAntiCheat.UseVisualStyleBackColor = true;
            this.btnVerifyAntiCheat.Click += new System.EventHandler(this.btnVerifyAntiCheat_Click);
            // 
            // btnQueryWorkshop
            // 
            this.btnQueryWorkshop.Location = new System.Drawing.Point(318, 44);
            this.btnQueryWorkshop.Name = "btnQueryWorkshop";
            this.btnQueryWorkshop.Size = new System.Drawing.Size(101, 23);
            this.btnQueryWorkshop.TabIndex = 5;
            this.btnQueryWorkshop.Text = "Query Workshop";
            this.btnQueryWorkshop.UseVisualStyleBackColor = true;
            this.btnQueryWorkshop.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btnUploadProgress
            // 
            this.btnUploadProgress.Location = new System.Drawing.Point(318, 73);
            this.btnUploadProgress.Name = "btnUploadProgress";
            this.btnUploadProgress.Size = new System.Drawing.Size(101, 21);
            this.btnUploadProgress.TabIndex = 6;
            this.btnUploadProgress.Text = "Upload Progress";
            this.btnUploadProgress.UseVisualStyleBackColor = true;
            this.btnUploadProgress.Click += new System.EventHandler(this.btnUploadProgress_Click);
            // 
            // btnGetSubscribed
            // 
            this.btnGetSubscribed.Location = new System.Drawing.Point(318, 100);
            this.btnGetSubscribed.Name = "btnGetSubscribed";
            this.btnGetSubscribed.Size = new System.Drawing.Size(101, 21);
            this.btnGetSubscribed.TabIndex = 7;
            this.btnGetSubscribed.Text = "Get Subscribed";
            this.btnGetSubscribed.UseVisualStyleBackColor = true;
            this.btnGetSubscribed.Click += new System.EventHandler(this.btnGetSubscribed_Click);
            // 
            // CheckPublishIDTimer
            // 
            this.CheckPublishIDTimer.Interval = 1000;
            this.CheckPublishIDTimer.Tick += new System.EventHandler(this.CheckPublishIDTimer_Tick);
            // 
            // CheckUploadTimer
            // 
            this.CheckUploadTimer.Interval = 1000;
            this.CheckUploadTimer.Tick += new System.EventHandler(this.CheckUploadTimer_Tick);
            // 
            // UploadProgress
            // 
            this.UploadProgress.Location = new System.Drawing.Point(12, 135);
            this.UploadProgress.Name = "UploadProgress";
            this.UploadProgress.Size = new System.Drawing.Size(776, 23);
            this.UploadProgress.TabIndex = 8;
            // 
            // CheckCallBacks
            // 
            this.CheckCallBacks.Enabled = true;
            this.CheckCallBacks.Interval = 2000;
            this.CheckCallBacks.Tick += new System.EventHandler(this.CheckCallBacks_Tick);
            // 
            // frmGameTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.UploadProgress);
            this.Controls.Add(this.btnGetSubscribed);
            this.Controls.Add(this.btnUploadProgress);
            this.Controls.Add(this.btnQueryWorkshop);
            this.Controls.Add(this.btnVerifyAntiCheat);
            this.Controls.Add(this.btnCheckOwnership);
            this.Controls.Add(this.btnSendStats);
            this.Controls.Add(this.txtDebug);
            this.Controls.Add(this.btnUploadWorkshop);
            this.Name = "frmGameTest";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUploadWorkshop;
        private System.Windows.Forms.TextBox txtDebug;
        private System.Windows.Forms.Button btnSendStats;
        private System.Windows.Forms.Button btnCheckOwnership;
        private System.Windows.Forms.Button btnVerifyAntiCheat;
        private System.Windows.Forms.Button btnQueryWorkshop;
        private System.Windows.Forms.Button btnUploadProgress;
        private System.Windows.Forms.Button btnGetSubscribed;
        private System.Windows.Forms.Timer CheckPublishIDTimer;
        private System.Windows.Forms.Timer CheckUploadTimer;
        private System.Windows.Forms.ProgressBar UploadProgress;
        private System.Windows.Forms.Timer CheckCallBacks;
    }
}

