using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;
using System.Runtime.InteropServices;
using System.Windows.Shapes;
using System.Drawing.Drawing2D;

namespace Dentique
{
    public partial class Base : Form
    {
        private List<Botones> menuButtons;
        private List<Botones> subMenuButtons;
        private Panel indicatorPanel;
        private Color selectedColor = Color.FromArgb(4, 85, 203);
        private Color defaultColor = Color.FromArgb(76, 81, 84);
        private Color panelSelectedColor = Color.FromArgb(1, 89, 228);
        private Color panelDefaultColor = Color.Silver;
        private Color panelDefaultColor2 = Color.White;
        private Color textSelectedColor = Color.FromArgb(1, 89, 228);
        private Color textDefaultColor = Color.Gray;
        private Botones activeButton = null;
        private Botones activeSubMenuButton = null; // Variable para rastrear el botón activo del submenú

        private Form activeForm = null; // Variable para almacenar el formulario activo

        private bool menuExpandido = true; // Estado del menú (expandido o contraído)
        private int anchoExpandido = 198;  // Ancho cuando está expandido
        private int anchoContraido = 62;   // Ancho cuando está contraído

        private bool subMenuActivo = false; // Estado del submenú "Pacientes"




        public Base()
        {
            InitializeComponent();
            InitializeMenu();
            InitializeSubMenu();


            // Asignar eventos de clic a los botones del menú
            btnHome.Click += (s, e) => OpenChildForm(new Home());
            btnPacientes.Click += (s, e) => OpenChildForm(new Pacientes());
            btnInsumos.Click += (s, e) => OpenChildForm(new Insumos());
            btnAgenda.Click += (s, e) => OpenChildForm(new Agenda());
            btnTratamientos.Click += (s, e) => OpenChildForm(new Tratamientos());
            btnFacturación.Click += (s, e) => OpenChildForm(new Facturación());
            btnPrescripciones.Click += (s, e) => OpenChildForm(new Prescripciones());
            btnFinanzas.Click += (s, e) => OpenChildForm(new Finanzas());
            btnSoporte.Click += (s, e) => OpenChildForm(new SoporteTécnico());
            btnInformación.Click += (s, e) => OpenChildForm(new Información());


        }


        //BARRA DE TÍTUTLO
        

        //Arrastrar formulario desde la barra superior
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void BarraSuperior_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void Base_Load(object sender, EventArgs e)
        {
            // 🔹 Posicionar correctamente el botón "Desplegar"
            btnDesplegar.Left = Menu.Width + ((Divisor1.Width - btnDesplegar.Width) / 2) - 1;

            // 🔹 Ajustar "pnlBarraDivisora" desde el inicio con márgenes de 10px
            pnlBarraDivisora.Width = Menu.Width - 20;
            pnlBarraDivisora.Left = 10;

            // 🔹 Configurar el ícono correcto al inicio
            btnDesplegar.IconChar = menuExpandido ? FontAwesome.Sharp.IconChar.ChevronLeft : FontAwesome.Sharp.IconChar.ChevronRight;
        }

        private void InitializeMenu()
        {
            // Lista de botones del menú principal
            menuButtons = new List<Botones> { btnHome, btnAgenda, btnTratamientos, btnFacturación, btnInsumos, btnPacientes, btnPrescripciones, btnFinanzas, btnSoporte, btnInformación };

            // Crear la línea indicadora
            indicatorPanel = new Panel
            {
                Size = new Size(4, 30), // Ancho de la línea
                BackColor = selectedColor, // Color de la línea
                Visible = false
            };

            // Agregar la línea directamente al formulario principal para que no se recorte
            this.Controls.Add(indicatorPanel);
            indicatorPanel.BringToFront(); // Asegurar que esté por encima de "Divisor2"

            // Asignar evento "Click" a cada botón del menú principal
            foreach (var btn in menuButtons)
            {
                btn.Click += MenuButton_Click;
            }

            // Asegurar que el subMenú esté oculto al inicio
            subMenú.Visible = false;
        }
        private void InitializeSubMenu()
        {
            // Lista de botones del submenú
            subMenuButtons = new List<Botones> { btnExpediente, btnDental, btnPlanDeTratamiento, btnFacturas};

            // Asignar evento "Click" a cada botón del submenú
            foreach (var btn in subMenuButtons)
            {
                btn.Click += SubMenuButton_Click;
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            if (sender is Botones selectedButton)
            {
                // 🔹 Restaurar color de todos los botones del menú principal
                foreach (var btn in menuButtons)
                {
                    btn.IconColor = defaultColor;
                    btn.TextColor = defaultColor;
                }

                // 🔹 Aplicar el color al botón seleccionado
                selectedButton.IconColor = selectedColor;
                selectedButton.TextColor = selectedColor;

                // 🔹 Si el botón seleccionado es "Pacientes"
                if (selectedButton == btnPacientes)
                {
                    // **Si el menú está contraído, NO abrir el submenú, pero mantener la barra indicadora**
                    if (!menuExpandido)
                    {
                        indicatorPanel.Visible = true;
                        indicatorPanel.Top = selectedButton.Top + (selectedButton.Height - indicatorPanel.Height) / 2;
                        indicatorPanel.Left = Menu.Right - (indicatorPanel.Width / 2);
                        activeButton = selectedButton;
                        return;
                    }

                    // **Alternar visibilidad del submenú solo si el menú está expandido**
                    subMenú.Visible = !subMenú.Visible;

                    // **Actualizar el ícono del botón desplegable**
                    btnDesplegarPacientes.IconChar = subMenú.Visible
                        ? FontAwesome.Sharp.IconChar.ChevronUp
                        : FontAwesome.Sharp.IconChar.ChevronDown;

                    // **Si el submenú se oculta, restablecer colores**
                    if (!subMenú.Visible)
                    {
                        foreach (var btn in subMenuButtons)
                        {
                            btn.TextColor = textDefaultColor;
                        }
                        ResetSubMenuIndicators();
                        activeSubMenuButton = null;
                    }
                }
                else
                {
                    // 🔹 Ocultar el submenú si otro botón del menú principal es presionado
                    subMenú.Visible = false;
                    btnDesplegarPacientes.IconChar = FontAwesome.Sharp.IconChar.ChevronDown;

                    // 🔹 Restaurar colores del submenú y su indicador
                    foreach (var btn in subMenuButtons)
                    {
                        btn.TextColor = textDefaultColor;
                    }
                    ResetSubMenuIndicators();
                    activeSubMenuButton = null;
                }

                // **🔹 Actualizar la posición de la barra indicadora**
                indicatorPanel.Height = selectedButton.IconSize;
                indicatorPanel.Top = selectedButton.Top + (selectedButton.Height - selectedButton.IconSize) / 2;
                indicatorPanel.Left = Menu.Right - (indicatorPanel.Width / 2);
                indicatorPanel.Visible = true;
                indicatorPanel.BringToFront();
                indicatorPanel.Refresh();

                // Guardar el botón activo
                activeButton = selectedButton;
            }
        }

        
        private void SubMenuButton_Click(object sender, EventArgs e)
        {
            if (sender is Botones selectedSubButton)
            {
                // 🔹 Restaurar el color de todos los botones del submenú antes de seleccionar uno nuevo
                foreach (var btn in subMenuButtons)
                {
                    btn.TextColor = textDefaultColor;
                }

                // 🔹 Aplicar el color al botón del submenú seleccionado
                selectedSubButton.TextColor = textSelectedColor;

                // 🔹 Restaurar los indicadores del submenú
                ResetSubMenuIndicators();

                // 🔹 Cambiar el color del indicador correspondiente
                if (selectedSubButton == btnExpediente)
                {
                    Indicador1.BackColor = panelSelectedColor;
                    Lx1.BackColor = panelSelectedColor;
                    Ly1.BackColor = panelSelectedColor;
                }
                else if (selectedSubButton == btnDental)
                {
                    Indicador2.BackColor = panelSelectedColor;
                    Lx2.BackColor = panelSelectedColor;
                    Ly2.BackColor = panelSelectedColor;
                }
                else if (selectedSubButton == btnPlanDeTratamiento)
                {
                    Indicador3.BackColor = panelSelectedColor;
                    Lx3.BackColor = panelSelectedColor;
                    Ly3.BackColor = panelSelectedColor;
                }
                else if (selectedSubButton == btnFacturas)
                {
                    Indicador4.BackColor = panelSelectedColor;
                    Lx4.BackColor = panelSelectedColor;
                    Ly4.BackColor = panelSelectedColor;
                }

                // Guardar el botón activo del submenú
                activeSubMenuButton = selectedSubButton;

                // **🔹 Asegurar que el botón "Pacientes" se mantenga resaltado**
                btnPacientes.IconColor = selectedColor;
                btnPacientes.TextColor = selectedColor;
            }
        }

        private void ResetSubMenuIndicators()
        {
            // Restaurar los colores de los indicadores del submenú
            Indicador1.BackColor = panelDefaultColor2;
            Lx1.BackColor = panelDefaultColor;
            Ly1.BackColor = panelDefaultColor;

            Indicador2.BackColor = panelDefaultColor2;
            Ly2.BackColor = panelDefaultColor;
            Lx2.BackColor = panelDefaultColor;

            Indicador3.BackColor = panelDefaultColor2;
            Ly3.BackColor = panelDefaultColor;
            Lx3.BackColor = panelDefaultColor;

            Indicador4.BackColor = panelDefaultColor2;
            Lx4.BackColor = panelDefaultColor;
            Ly4.BackColor = panelDefaultColor;
        }



        private void btnHome_Click(object sender, EventArgs e)
        {
            
        }

        private void OpenChildForm(Form childForm)
        {
            try
            {
                // Cerrar el formulario anterior si existe
                if (activeForm != null)
                {
                    activeForm.Close();
                    activeForm.Dispose(); // Liberar memoria
                }

                // Configurar el nuevo formulario
                activeForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;

                // Agregar el formulario al contenedor
                Contenedor.Controls.Clear();
                Contenedor.Controls.Add(childForm);
                Contenedor.Tag = childForm;
                childForm.BringToFront();
                childForm.Show();

                // 🔹 Forzar la actualización visual del formulario
                childForm.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el formulario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDesplegar_Click(object sender, EventArgs e)
        {
            menuExpandido = !menuExpandido;
            Menu.Width = menuExpandido ? anchoExpandido : anchoContraido;

            // Ajustar la posición del botón "Desplegar"
            btnDesplegar.Left = Menu.Width + ((Divisor1.Width - btnDesplegar.Width) / 2) - 1;

            // Ajustar el tamaño y la posición de pnlBarraDivisora al contraer/expandir el menú
            pnlBarraDivisora.Width = menuExpandido ? anchoExpandido - 20 : anchoContraido - 20;
            pnlBarraDivisora.Left = (Menu.Width - pnlBarraDivisora.Width) / 2;

            if (!menuExpandido)
            {
                // 🔹 **Guardar el estado del submenú antes de contraer**
                subMenuActivo = subMenú.Visible;
                subMenú.Visible = false;
                btnDesplegarPacientes.IconChar = FontAwesome.Sharp.IconChar.ChevronDown;
            }
            else
            {
                // 🔹 **Restaurar el estado del submenú si estaba abierto antes**
                subMenú.Visible = subMenuActivo;
                btnDesplegarPacientes.IconChar = subMenuActivo ?
                    FontAwesome.Sharp.IconChar.ChevronUp :
                    FontAwesome.Sharp.IconChar.ChevronDown;
            }

            // 🔹 **Actualizar ícono de btnDesplegar**
            btnDesplegar.IconChar = menuExpandido ?
                FontAwesome.Sharp.IconChar.ChevronLeft :
                FontAwesome.Sharp.IconChar.ChevronRight;

            // 🔹 Restaurar la posición de la barra indicadora correctamente
            if (activeButton != null)
            {
                indicatorPanel.Left = Menu.Right - (indicatorPanel.Width / 2);
                indicatorPanel.Visible = true;
            }

            ActualizarMenuVisual(menuExpandido);
        }
        private void ActualizarMenuVisual(bool expandido)
        {
            foreach (var btn in menuButtons)
            {
                if (expandido)
                {
                    btn.Text = btn.Tag?.ToString() ?? ""; // Restaurar texto desde Tag si existe
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                    btn.Padding = new Padding(10, 0, 0, 0);
                }
                else
                {
                    btn.Tag = btn.Text; // Guardar el texto antes de ocultarlo
                    btn.Text = ""; // Ocultar texto sin mover el ícono
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                    btn.Padding = new Padding(10, 0, 0, 0);
                }
            }

            // 🔹 Restaurar color del botón "Pacientes" si está seleccionado
            if (activeButton == btnPacientes)
            {
                btnPacientes.IconColor = selectedColor;
                btnPacientes.TextColor = selectedColor;
            }

            // 🔹 Restaurar el estado del submenú de "Pacientes"
            if (expandido)
            {
                subMenú.Visible = subMenuActivo; // **Mantiene abierto si estaba abierto antes**
                btnDesplegarPacientes.IconChar = subMenuActivo ?
                    FontAwesome.Sharp.IconChar.ChevronUp :
                    FontAwesome.Sharp.IconChar.ChevronDown;
            }

            // 🔹 Restaurar selección del botón del submenú si había uno activo
            if (activeSubMenuButton != null)
            {
                activeSubMenuButton.TextColor = textSelectedColor;
            }

            // 🔹 Ajustar la posición de la barra indicadora si está activo
            if (activeButton != null)
            {
                indicatorPanel.Left = Menu.Right - (indicatorPanel.Width / 2);
                indicatorPanel.Visible = true;
            }



        }
    }   
}
