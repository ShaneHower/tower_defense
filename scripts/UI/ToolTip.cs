namespace GameNamespace.UI
{
    using System;
    using System.Threading.Tasks;
    using Godot;

    public partial class ToolTip : Control
    {
        private Panel toolTip;
        private VBoxContainer labelBox;

        public override void _Ready()
        {
            toolTip = GetNode<Panel>("Panel");
            labelBox = new VBoxContainer();
            toolTip.AddChild(labelBox);
            BuildTowerMessage();

            _ = ResizePanel();
        }

        private void BuildTowerMessage()
        {
            RichTextLabel towerName = UITools.Instance.CreateRichTextLabel(text:"[b][font_size=16]ICE TOWER[/font_size][/b]", fontSize:15);
            labelBox.AddChild(towerName);
            towerName.HorizontalAlignment = HorizontalAlignment.Center;
            towerName.SizeFlagsVertical = SizeFlags.Fill;
            towerName.CustomMinimumSize = new Vector2(60, 15);
            towerName.BbcodeEnabled = true;

            RichTextLabel towerDescription = UITools.Instance.CreateRichTextLabel(text:"The ice tower shoots ice shards that damage and slow a single target for 1.5 seconds.", fontSize:15);
            labelBox.AddChild(towerDescription);
            towerDescription.CustomMinimumSize = new Vector2(120, 15);
            towerDescription.ScrollActive = false;
            towerDescription.FitContent = true;

            BuildStats(name:"Cost", value:"200");
            BuildStats(name:"Damage", value:"20");
        }
        private void BuildStats(string name, string value)
        {
            RichTextLabel labelName = UITools.Instance.CreateRichTextLabel(text:$"{name.ToUpper()}: [color=#ad2f45]{value}[/color]", fontSize:15);
            labelBox.AddChild(labelName);
            labelName.CustomMinimumSize = new Vector2(45, 15);
            labelName.BbcodeEnabled = true;
        }

        private async Task ResizePanel()
        {
            // This is an important command that I just learned about. QueueSort will tell the object that something changed,
            // recalculate the layer of your children.  This allows the code to catch up with whats been changed and will get
            // you an accurate measurement of whats been changed.
            labelBox.QueueSort();
            await ToSignal(GetTree(), "process_frame");
            toolTip.Size = labelBox.GetCombinedMinimumSize();
        }
    }
}
