﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba.Model;
using UberFrba.CapaDAO;

namespace UberFrba.Abm_Automovil
{
    public partial class ModificacionAutomovil : FormBase
    {
        public ModificacionAutomovil()
        {
            InitializeComponent();
            DataTable marcas = DAOAutomovil.getMarcas();

            comboBoxMarca.DisplayMember = "MARCA_NOMBRE";
            comboBoxMarca.DataSource = marcas;

            textBoxChofer.Visible = false;
            checkBoxEstado.Visible = false;
            label5.Visible = false;
            label7.Visible = false;

        }

        private bool ValidarChofer(string chofer, string patente)
        {
            if (chofer == null) {Mensaje_Error("El chofer se encuentra vacio"); return false;}
            if (DAOAutomovil.choferAsignado(chofer, patente) == true) { Mensaje_Error("El chofer ya posee auto"); return false; }
            return true;
        }

        private void buttonAceptar_Click(object sender, EventArgs e)
        {

            string patente = comboBoxPatente.Text;
            int estado = Convert.ToInt32(checkBoxEstado.Checked);
            string chofer = textBoxChofer.Text;

            var resultado = Mensaje_Pregunta("¿Está seguro que desea modificar el automovil?", "Modificar Automovil");
            if (resultado == DialogResult.Yes)
            {
                if (!ValidarChofer(chofer, patente)) { Mensaje_Error("Modificacion no valida"); }
                else
                {
                    DAOAutomovil.modificarAutomovilPorPatente(patente, estado, chofer);
                    this.actualizarTurnosAutomovil(patente);
                    Mensaje_OK("El automovil fue modificado");
                    this.Close();
                }
            }
        }
        private void actualizarTurnosAutomovil(string patente)
        {
            DataRow row;
            int cont;
            for (cont = 0; cont < Turno.Items.Count; cont++)
            {
                int estadoTurno = Convert.ToInt32(Turno.GetItemChecked(cont));
                if (estadoTurno == 0)
                {
                    row = ((DataRowView)this.Turno.Items[cont]).Row;
                    string detalle = (row[this.Turno.ValueMember]).ToString();
                    DAOAutomovil.modificarTurnoAutomovilPorPatente(patente, detalle, estadoTurno);
                }
            }

            for (cont = 0; cont < TurnoDeshabilitado.Items.Count; cont++)
            {
                int estadoTurno = Convert.ToInt32(TurnoDeshabilitado.GetItemChecked(cont));
                if (estadoTurno == 1)
                {
                    row = ((DataRowView)this.TurnoDeshabilitado.Items[cont]).Row;
                    string detalle = (row[this.TurnoDeshabilitado.ValueMember]).ToString();
                    DAOAutomovil.modificarTurnoAutomovilPorPatente(patente, detalle, estadoTurno);
                }
            }

        }

        private void cargarTurnosDeshabilitados(DataTable table, int estado)
        {
            int cont = 0;
            TurnoDeshabilitado.DataSource = table;
            try
            {
                DataRow row = table.Rows[cont];
                while (row != null)
                {
                    TurnoDeshabilitado.ValueMember = "TURNO_DESCRIPCION";
                    TurnoDeshabilitado.SetItemChecked(cont, Convert.ToBoolean(estado));
                    cont++;
                    try { row = table.Rows[cont]; }
                    catch { row = null; }
                }
            }catch
            { Mensaje_OK("No existen turnos a asignar");}
        }

        private void cargarTurnosHabilitados(DataTable table, int estado)
        {
            int cont = 0;
            Turno.DataSource = table;
            try
            {
            DataRow row = table.Rows[cont];
            while (row != null)
            {
                Turno.ValueMember = "TURNO_DESCRIPCION";
                Turno.SetItemChecked(cont, Convert.ToBoolean(estado));
                cont++;
                try { row = table.Rows[cont]; }
                catch { row = null; }
            }
            }
            catch
            { Mensaje_OK("No existen turnos asignados"); }
        }

        private void buttonSeleccionar_Click(object sender, EventArgs e)
        {
            DataTable table = DAOAutomovil.getEstadoYChofer(comboBoxPatente.Text);
                DataRow row = table.Rows[0];
                textBoxChofer.Visible = true;
                checkBoxEstado.Visible = true;
                label5.Visible = true;
                label7.Visible = true;
                int estado = Convert.ToInt32(row["COCHE_ESTADO"]);
                string chofer = row["PERSONA"] as string;
                textBoxChofer.Text = chofer;
                if (estado == 1)
                {
                    checkBoxEstado.Visible = false;
                    label7.Visible = false;
                    textBoxChofer.Enabled = true;
                    checkBoxEstado.Checked = true;
                    labelTurnoHabilitado.Visible = true;
                    Turno.Visible = true;
                    label4.Visible = true;
                    TurnoDeshabilitado.Visible = true;
                    this.cargarTurnosHabilitados(DAOTurno.getTurnoAsignadoACoche(comboBoxPatente.Text), 1);
                    this.cargarTurnosDeshabilitados(DAOTurno.getTurnoNoAsignadoACoche(comboBoxPatente.Text), 0);
                }
                else
                {
                    checkBoxEstado.Visible = true;
                    label7.Visible = true;
                    textBoxChofer.Enabled = false;
                }
                    buttonAceptar.Visible = true;
                    
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox_Marca_SelectedIndexChanged(object sender, EventArgs e)
        {

            DataTable modelos = DAOAutomovil.getModelos(comboBoxMarca.Text);
            comboBoxModelo.DisplayMember = "COCHE_MODELO_CODIGO";
            comboBoxModelo.DataSource = modelos;

        }

        private void comboBox_Modelo_SelectedIndexChanged(object sender, EventArgs e)
        {

            DataTable patentes = DAOAutomovil.getPatentes(comboBoxModelo.Text);
            comboBoxPatente.DisplayMember = "COCHE_PATENTE";
            comboBoxPatente.DataSource = patentes;

        }

    }
}
