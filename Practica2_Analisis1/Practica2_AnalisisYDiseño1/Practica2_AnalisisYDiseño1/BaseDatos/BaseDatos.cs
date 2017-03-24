using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Practica2_AnalisisYDiseño1
{
    public class BaseDatos
    {
        private static string CadenaConex = "server=localhost;port=3306;database=practica2_analisis1;Uid=root;pwd=root";
        public SqlConnection Conectar()
        {
            SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["EDUARDOCASEROS"].ConnectionString);

            return Con;

        }

        //******************************************** METODO PARA CONECTARCE A LA BASE DE DATOS MYSQL ********************************************************************
        public MySqlConnection ObtenerConexion()
        {
            MySqlConnection Conex = new MySqlConnection("server=localhost;port=3306;database=practica2_analisis1;Uid=root;pwd=root");
            
            return Conex;
        }

        //**************************************************** FUNCION PARA REGISTRAR UN NUEVO USUARIO **************************************************************+
        public static int RegistroUsuario(string nombre, string nombreUsuario, string correo, string contrasena)
        {


            MySqlConnection Conex = new MySqlConnection(CadenaConex);
            Conex.Open();
            MySqlDataAdapter Comando = new MySqlDataAdapter("SELECT Insertar_Usuario('"+nombre+"','"+nombreUsuario+"','"+correo+"','"+contrasena+"') AS bandera", Conex);
            DataTable TablaEnviar = new DataTable();
            Comando.Fill(TablaEnviar);
            Conex.Close();

            return Int32.Parse(TablaEnviar.Rows[0]["bandera"].ToString());
    
            
            /*
            int RetornoSP = 3;
            SqlConnection Conex = Conectar();
            SqlTransaction Transaccion = null;
            try
            {
                Conex.Open();
                Transaccion = Conex.BeginTransaction(System.Data.IsolationLevel.Serializable);
                SqlCommand Comando = new SqlCommand("Insertar_Usuario", Conex, Transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Clear();
                Comando.Parameters.AddWithValue("@nombre", nombre);
                Comando.Parameters.AddWithValue("@usuario", nombreUsuario);
                Comando.Parameters.AddWithValue("@correo", correo);
                Comando.Parameters.AddWithValue("@contra", contrasena);

                //Variable para obtener el valor retornado por el SP
                SqlParameter ValorRetorno = new SqlParameter("@mensaje", SqlDbType.Int);

                //Se le asigna el valor del retorno al SP
                ValorRetorno.Direction = ParameterDirection.Output;
                Comando.Parameters.Add(ValorRetorno);

                //Se ejecuta el Query
                Comando.ExecuteNonQuery();
                RetornoSP = Convert.ToInt32(ValorRetorno.Value);
            }
            catch (Exception e)
            {
                RetornoSP = 3;   //Error al conectar con la Base de Datos
            }
            finally
            {
                if (RetornoSP == 1)
                {
                    Transaccion.Commit();
                    Conex.Close();
                    
                }
                else if (RetornoSP == 0)
                {
                    Transaccion.Rollback();
                    Conex.Close();
                   
                }
            }
            */

            
        }

        //********************************************** METODO PARA LOGUEARSE ************************************************
        public static int LoginUsuario(int cuenta, string usuario, string contrasena)
        {
            MySqlConnection Conex = new MySqlConnection(CadenaConex);
            Conex.Open();
            MySqlDataAdapter Comando = new MySqlDataAdapter("select LoginTrue("+cuenta+",'"+usuario+"','"+contrasena+"') AS bandera", Conex);
            DataTable TablaEnviar = new DataTable();
            Comando.Fill(TablaEnviar);
            Conex.Close();

            return Int32.Parse(TablaEnviar.Rows[0]["bandera"].ToString());
            
            /*
            SqlConnection Conex = Conectar();
            Conex.Open();
            SqlDataAdapter Comando = new SqlDataAdapter("SELECT * FROM Login_usuario(" + cuenta+",'"+usuario+"','"+contrasena+"')",Conex);
            DataTable TablaEnviar = new DataTable();
            Comando.Fill(TablaEnviar);
            Conex.Close();
            return TablaEnviar;
             */

        }
        //************************************************* FUNCION PARA REGISTRAR UNA CUENTA ******************************************
        public static int RegistrarCuenta(int cuenta,float monto,string usua)
        {
           
            DataTable TablaEnviar = new DataTable();
            MySqlConnection Conex = new MySqlConnection(CadenaConex);
            try
            {
                
                Conex.Open();
                MySqlDataAdapter Comando = new MySqlDataAdapter("SELECT Ingreso_Cuenta("+cuenta+","+monto+",'"+usua+"') as bandera", Conex);
                Comando.Fill(TablaEnviar);
                Conex.Close();
            }catch(Exception e)
            {
                Conex.Close();
                return 2;
            
            }
            return Int32.Parse(TablaEnviar.Rows[0]["bandera"].ToString());

        }
        public static DataTable DatosUsuario(int cuenta)
        {
            MySqlConnection Conex = new MySqlConnection(CadenaConex);
            DataTable TablaEnviar = new DataTable();
            try
            {

                Conex.Open();
                String Inst = "DatosUsuario";
                MySqlCommand Comando = new MySqlCommand(Inst, Conex);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@cuenta", cuenta);               


                MySqlDataReader Tabla = Comando.ExecuteReader();

                TablaEnviar.Load(Tabla);
                Tabla.Close();

            }
            catch (Exception e)
            {
                Conex.Close();
                return TablaEnviar;
            }

            Conex.Close();
            return TablaEnviar;
        }

        //******************************************* METODO PARA VALIDAR DATOS DE PAGOS ********************************************
        public static bool ValidarDatosPagos(string cuenta,string monto)
        {
            try
            {
                float.Parse(monto);
                Int32.Parse(cuenta);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //******************************************** FUNCION PARA REALIZAR UN PAGO **********************************************************
        public static int RealizarPago(int CuentaLog,float Monto, int CuentaSer, int tipoSer)
        {
            float SaldoRestante = ConsultarSaldo(CuentaLog);
            if (SaldoRestante < Monto)
            {
                return -1;
            }
            else
            {
                //Se realiza la transaccion
                MySqlConnection Conex = new MySqlConnection(CadenaConex);
                DataTable TablaEnviar = new DataTable();
                try
                {

                    Conex.Open();
                    String Inst = "pagoServicio";
                    MySqlCommand Comando = new MySqlCommand(Inst, Conex);
                    Comando.CommandType = CommandType.StoredProcedure;
                    Comando.Parameters.AddWithValue("@numero", CuentaLog);
                    Comando.Parameters.AddWithValue("@numero_servicio", CuentaSer);
                    Comando.Parameters.AddWithValue("@mon", Monto);
                    Comando.Parameters.AddWithValue("@tipo_servicio", tipoSer);
                    MySqlDataReader Tabla = Comando.ExecuteReader();

                    TablaEnviar.Load(Tabla);
                    Tabla.Close();
                    Conex.Close();
                    return 1;
                    

                }
                catch (Exception e)
                {
                    Conex.Close();
                    return -1;
                }
            

            }
            
        
        }

        //**************************************** FUNCION PARA CONSULTAR SALDO DE UNA CUENTA *************************************************+
        public static float ConsultarSaldo(int cuenta)
        {
            MySqlConnection Conex = new MySqlConnection(CadenaConex);
            DataTable TablaEnviar = new DataTable();
            try
            {
                Conex.Open();
                MySqlDataAdapter Comando = new MySqlDataAdapter("SELECT consultarSaldo(" + cuenta + ") as Sald", Conex);
                Comando.Fill(TablaEnviar);
                Conex.Close();
                
            }
            catch
            {
                Conex.Close();
                return -1;
            }

            if (TablaEnviar.Rows.Count == 0)
                return -1;

            return float.Parse(TablaEnviar.Rows[0]["Sald"].ToString());

        }

        //************************************ FUNCION PARA TRANSFERIR SALDO ************************************************
        public static int Transferencia(int cuentaOrigen,int cuentaDestino,float monto)
        {
            if (ExisteCuenta(cuentaDestino) == 0)
            {
                return 2;
            }
            else
            {
                float SaldoRestante = ConsultarSaldo(cuentaOrigen);
                if (SaldoRestante < monto)
                {
                    return -1;
                }
                else
                {
                    //Se realiza la transaccion
                    MySqlConnection Conex = new MySqlConnection(CadenaConex);
                    DataTable TablaEnviar = new DataTable();
                    try
                    {

                        Conex.Open();
                        String Inst = "transferir";
                        MySqlCommand Comando = new MySqlCommand(Inst, Conex);
                        Comando.CommandType = CommandType.StoredProcedure;
                        Comando.Parameters.AddWithValue("@numero", cuentaOrigen);
                        Comando.Parameters.AddWithValue("@numero_transferido", cuentaDestino);
                        Comando.Parameters.AddWithValue("@mon", monto);
                        Comando.Parameters.AddWithValue("@descripcion", "transferencia");
                        MySqlDataReader Tabla = Comando.ExecuteReader();

                        TablaEnviar.Load(Tabla);
                        Tabla.Close();
                        Conex.Close();
                        return 1;


                    }
                    catch (Exception e)
                    {
                        Conex.Close();
                        return -1;
                    }


                }
            
            }


            
        }

        //************************************* FUNCION PARA VALIDAR QUE UNA CUENTA EXISTE *********************************
        public static int ExisteCuenta(int Cuenta)
        {
           
            MySqlConnection Conex = new MySqlConnection(CadenaConex);
            Conex.Open();
            MySqlDataAdapter Comando = new MySqlDataAdapter("select usuarioExistente("+Cuenta+") as ban", Conex);
            DataTable TablaEnviar = new DataTable();
            Comando.Fill(TablaEnviar);
            Conex.Close();

            return Int32.Parse(TablaEnviar.Rows[0]["ban"].ToString());
            
            
            
        }

        // ********************************* FUNCION REALIZAR UN DEBITO ***********************************************************
        public static int Debitar(int cuentaOrigen, int cuentaDestino, float monto,string descripcion)
        {
            if (ExisteCuenta(cuentaDestino) == 0)
            {
                return 2;
            }
            else
            {
                float SaldoRestante = ConsultarSaldo(cuentaDestino);
                if (SaldoRestante < monto)
                {
                    return -1;
                }
                else
                {
                    //Se realiza la transaccion
                    MySqlConnection Conex = new MySqlConnection(CadenaConex);
                    DataTable TablaEnviar = new DataTable();
                    try
                    {

                        Conex.Open();
                        String Inst = "debito";
                        MySqlCommand Comando = new MySqlCommand(Inst, Conex);
                        Comando.CommandType = CommandType.StoredProcedure;
                        Comando.Parameters.AddWithValue("@numero_debitador", cuentaOrigen);
                        Comando.Parameters.AddWithValue("@numero_debitado", cuentaDestino);
                        Comando.Parameters.AddWithValue("@mon", monto);
                        Comando.Parameters.AddWithValue("@descripcion", descripcion);
                        MySqlDataReader Tabla = Comando.ExecuteReader();

                        TablaEnviar.Load(Tabla);
                        Tabla.Close();
                        Conex.Close();
                        return 1;


                    }
                    catch (Exception e)
                    {
                        Conex.Close();
                        return -1;
                    }


                }

            }
        }

        // ****************************************** FUNCION PARA REALIAR UN CREDITO ******************************************************
        public static int Credito(int cuentaOrigen, int cuentaDestino, float monto, string descripcion)
        {

            if (ExisteCuenta(cuentaDestino) == 0)
            {
                return 2;
            }
            else
            {
                float SaldoRestante = ConsultarSaldo(cuentaDestino);
                if (SaldoRestante < monto)
                {
                    return -1;
                }
                else
                {
                    //Se realiza la transaccion
                    MySqlConnection Conex = new MySqlConnection(CadenaConex);
                    DataTable TablaEnviar = new DataTable();
                    try
                    {

                        Conex.Open();
                        String Inst = "credito";
                        MySqlCommand Comando = new MySqlCommand(Inst, Conex);
                        Comando.CommandType = CommandType.StoredProcedure;
                        Comando.Parameters.AddWithValue("@numero_acreditador", cuentaOrigen);
                        Comando.Parameters.AddWithValue("@numero_acreditado", cuentaDestino);
                        Comando.Parameters.AddWithValue("@mon", monto);
                        Comando.Parameters.AddWithValue("@descripcion", descripcion);
                        MySqlDataReader Tabla = Comando.ExecuteReader();

                        TablaEnviar.Load(Tabla);
                        Tabla.Close();
                        Conex.Close();
                        return 1;


                    }
                    catch (Exception e)
                    {
                        Conex.Close();
                        return -1;
                    }


                }

            }
        
        }





    }
}