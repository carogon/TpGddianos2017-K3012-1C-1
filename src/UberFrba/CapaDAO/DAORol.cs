﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba.Model;

namespace UberFrba.CapaDAO
{
    class DAORol : SqlConnector
    {

        public static RolUsuario getRolUsuario(int id, int tipo)
        {
            RolUsuario rol;

            DataTable table = retrieveDataTable("GET_ROLES_USUARIO_POR_ID", id, tipo);

            return rol = dataRowToPersona(table.Rows[0]);
        }

     

        public static RolUsuario dataRowToPersona(DataRow row)
        {
            RolUsuario rol = new RolUsuario (row["ROL_USUA_USERNAME"] as string,
                               Convert.ToInt32(row["ROL_ROL_ID"]),
                               Convert.ToInt32(row["ROL_ROL_ESTADO"]));
            return rol;

        }

        public static DataTable getRoles()
        {
             
            return retrieveDataTable("GET_ROLES");
            
        }

        public static void bajaRolSeleccionado (string rol)
        {
            executeProcedure("BAJA_ROL", rol);
        }

        

    }
}
