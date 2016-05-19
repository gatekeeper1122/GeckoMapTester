using System;
using System.Windows.Forms;

namespace GeckoMapTester
{
    public partial class Form1 : Form
    {



        public TCPGecko Gecko;
        string MapAscii;
        uint address;
        string extrasite;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String[] arguments = Environment.GetCommandLineArgs();
            try
            {
                extrasite = arguments[1];
                MessageBox.Show("Using custom python library dir: \n\n" + extrasite, "", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
            catch (Exception exc)
            {
                extrasite = @"C:\Python27\Lib";
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Gecko = new TCPGecko(IPBox.Text, 7331);
            try {
                Gecko.Connect();
            }catch(ETCPGeckoException exc)
            {
                MessageBox.Show("Connection to the TCPGecko failed: \n\n" + exc.Message, "Connection failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            groupBox2.Enabled = true;
            DisconnButton.Enabled = true;
            ConnectButton.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Gecko.Disconnect();
            groupBox2.Enabled = false;
            DisconnButton.Enabled = false;
            ConnectButton.Enabled = true;
        }

        /* hacky, needs ironpython and messes up padding
        private void writestr(uint address, string str)
        {
            ScriptEngine engine = Python.CreateEngine();
            try {
                ICollection<string> paths = engine.GetSearchPaths();


                paths.Add(extrasite);
                //paths.Add(Environment.CurrentDirectory);

                engine.SetSearchPaths(paths);

                var script = @"import struct
if type(string) != bytes: string = bytes(string, ""UTF - 8"")
if len(string) % 4: string += bytes((4 - (len(string) % 4)) * b""\x00"")
pos = 0
for x in range(int(len(string) / 4)):
    me.PokeDummy(address, struct.unpack("">I"", string[pos:pos + 4])[0])
    address += 4;
    pos += 4";

                ScriptScope scope = engine.CreateScope();

                //Test class with a method that prints to the screen.
                scope.SetVariable("me", this);
                scope.SetVariable("string", str);
                scope.SetVariable("address", address);

                ScriptSource source = engine.CreateScriptSourceFromString(script, Microsoft.Scripting.SourceCodeKind.Statements);


                source.Execute(scope);

            }catch(Exception exc)
            {
                MessageBox.Show("IronPython Operation failed: \n\n" + engine.GetService<ExceptionOperations>().FormatException(exc) + "\n\nCheck your lib directory!", "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        */

        private void PokeAllMaps(string NewMapName)
        {
            writeStringSimple(0x12AEE594, NewMapName);
            writeStringSimple(0x12AEE51C, NewMapName);
            writeStringSimple(0x12AEE4A4, NewMapName);
            writeStringSimple(0x12AEE42C, NewMapName);
            writeStringSimple(0x12AEE3B4, NewMapName);
            writeStringSimple(0x12AEE33C, NewMapName);
            writeStringSimple(0x12AEE2C4, NewMapName);
            writeStringSimple(0x12AEE24C, NewMapName);
            writeStringSimple(0x12AEE1D4, NewMapName);
            writeStringSimple(0x12AEE15C, NewMapName);
            writeStringSimple(0x12AEE0E4, NewMapName);
            writeStringSimple(0x12AEE06C, NewMapName);
            writeStringSimple(0x12AEDFF4, NewMapName);
            writeStringSimple(0x12AEDF7C, NewMapName);
            writeStringSimple(0x12AEDF04, NewMapName);
            writeStringSimple(0x12AEDE8C, NewMapName);

            writeStringSimple(0x12B4BA3C, NewMapName);
            writeStringSimple(0x12B4BCC4, NewMapName);
            writeStringSimple(0x12B4BF98, NewMapName);
            writeStringSimple(0x12B4C1D4, NewMapName);
            writeStringSimple(0x12B4C45C, NewMapName);
            writeStringSimple(0x12B4C6E4, NewMapName);
            writeStringSimple(0x12B4C96C, NewMapName);
            writeStringSimple(0x12B4CBF4, NewMapName);
            writeStringSimple(0x12B4CE7C, NewMapName);
            writeStringSimple(0x12B4D104, NewMapName);
            writeStringSimple(0x12B4D38C, NewMapName);
            writeStringSimple(0x12B4D614, NewMapName);
        }

        private void PokeButton_Click(object sender, EventArgs e)
        {
            PokeAllMaps(NameCBox.Text);
            MessageBox.Show("Success!", "GeckoTool", MessageBoxButtons.OK);
        }

        /* gah
        public void PokeDummy(int add, int val)
        {

            //MessageBox.Show("add: " + add + "\nval: " + val, "", MessageBoxButtons.OK, MessageBoxIcon.Stop);        
            try {
                uint test1 = Convert.ToUInt32(add);
                uint test2 = Convert.ToUInt32(val);

                try
                {
                    Gecko.poke(test1, test2);
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Writing values to RAM failed: \n\n" + exc.Message, "Poke failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch(Exception exc)
            {
                MessageBox.Show("Writing values to RAM failed: \n\n" + exc.Message, "Poke failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        */
        private void writeString(string s, uint offset)
        {
            uint push = 0;
            int pos = 0;
            if (offset % 4 != 0)
            {
                push = s[pos++];
                offset -= offset % 4;
                Gecko.poke(offset, push);
                offset += 4;
            }
            for (; pos < s.Length; pos += 2, offset += 4)
            {
                if (pos + 1 == s.Length)
                {
                    push = (uint)(s[pos] << 16) + (Gecko.peek(offset) & 0xFF);
                }
                push = (uint)(s[pos] << 16) + s[pos + 1];
                Gecko.poke(offset, push);
            }
        }

        private void writeStringSimple(uint offset, string s)
        {
            writeStringSimple(offset, s, s.Length);
        }
        private void writeStringSimple(uint offset, string s, int length)
        {
            uint push = 0;
            int pos = 0;
            if (offset % 4 != 0)
            {
                for (int i = 0; i < offset % 4; i++)
                {
                    push = push << 8 | s[pos++];
                }
                if (offset % 4 == 1)
                {
                    push = Gecko.peek(offset - offset % 4) & 0xFF000000 | push;
                }
                if (offset % 4 == 2)
                {
                    push = Gecko.peek(offset - offset % 4) & 0xFFFF0000 | push;
                }
                if (offset % 4 == 3)
                {
                    push = Gecko.peek(offset - offset % 4) & 0xFFFFFF00 | push;
                }
                Gecko.poke(offset, push);
                offset += offset % 4;
            }
            for (; pos < s.Length; offset += 4)
            {
                push = 0;
                if (pos + 1 == s.Length)
                {
                    push = (uint)s[pos++] << 24 | Gecko.peek(offset) & 0x00FFFFFF;
                    Gecko.poke(offset, push);
                    offset += 1;
                    break;
                }
                if (pos + 2 == s.Length)
                {
                    push = s[pos++];
                    push = push << 8 | s[pos++];
                    push = push << 16 | Gecko.peek(offset) & 0x0000FFFF;
                    Gecko.poke(offset, push);
                    offset += 2;
                    break;
                }
                if (pos + 3 == s.Length)
                {
                    push = s[pos++];
                    push = push << 8 | s[pos++];
                    push = push << 8 | s[pos++];
                    push = push << 8 | Gecko.peek(offset) & 0x000000FF;
                    Gecko.poke(offset, push);
                    break;
                }
                for (int i = 0; i < 4; i++)
                {
                    push = push << 8 | s[pos++];
                }
                Gecko.poke(offset, push);
            }
            for (; pos < length; offset += 4, pos += 4)
            {
                if (pos % 4 == 1)
                {
                    Gecko.poke(offset, Gecko.peek(offset) & 0xFF000000);
                    pos--;
                    continue;
                }
                if (pos % 4 == 2)
                {
                    Gecko.poke(offset, Gecko.peek(offset) & 0xFFFF0000);
                    pos--; pos--;
                    continue;
                }
                if (pos % 4 == 3)
                {
                    Gecko.poke(offset, Gecko.peek(offset) & 0xFFFFFF00);
                    pos--; pos--; pos--;
                    continue;
                }
                if (pos + 1 == length)
                {
                    push = Gecko.peek(offset) & 0x00FFFFFF;
                    Gecko.poke(offset, push);
                    offset += 1;
                    pos++;
                    break;
                }
                if (pos + 2 == length)
                {
                    push = Gecko.peek(offset) & 0x0000FFFF;
                    Gecko.poke(offset, push);
                    offset += 2;
                    pos += 2;
                    break;
                }
                if (pos + 3 == length)
                {
                    push = Gecko.peek(offset) & 0x000000FF;
                    Gecko.poke(offset, push);
                    offset += 3;
                    pos += 3;
                    break;
                }
                Gecko.poke(offset, 0);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
