using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Graphics;
using Android.Widget;
using Plugin.Media;
using System;
using System.IO;
using Plugin.CurrentActivity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Android.Runtime;

namespace Agencia_de_Viajes__PF_PDM_ITSN601_
{
    [Activity(Label = "C_Clientes")]
    public class C_Clientes : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Clientes);
            SupportActionBar.Hide();

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            var txtNombre = FindViewById<EditText>(Resource.Id.edtxtnombre);
            var txtCorreo = FindViewById<EditText>(Resource.Id.edtxtcorreo);
            var txtCelular = FindViewById<EditText>(Resource.Id.edtxtcelular);
            var btnGuardar = FindViewById<Button>(Resource.Id.btnguardar);
            var img1 = FindViewById<ImageView>(Resource.Id.imageView);
            var img2 = FindViewById<ImageView>(Resource.Id.imageView2);
            var img3 = FindViewById<ImageView>(Resource.Id.imageView3);
            var img4 = FindViewById<ImageView>(Resource.Id.imageView4);

            img1.SetImageResource(Resource.Drawable.registro);
            img2.SetImageResource(Resource.Drawable.nombre);
            img3.SetImageResource(Resource.Drawable.telefono);
            img4.SetImageResource(Resource.Drawable.correo);

            btnGuardar.Click += async delegate
            {
                try
                {
                    var CuentadeAlmacenamiento = CloudStorageAccount.Parse
                    ("DefaultEndpointsProtocol=https;AccountName=programacionparamovil;AccountKey=1MHCQg8Cmym2unWhGkWu3SY0ctyppH8vfnYNOy3lRjUAzA9TVJnHycMnduZtvb7dacoN4/rJ02B0dg2qP520UQ==;EndpointSuffix=core.windows.net");


                    //Almacenar en TablaNoSql clientes
                    var TablaNoQSL = CuentadeAlmacenamiento.CreateCloudTableClient();
                    var Coleccion = TablaNoQSL.GetTableReference("clientes");
                    await Coleccion.CreateIfNotExistsAsync();
                    var clientes = new Clientes("Clientes", txtNombre.Text);
                    clientes.Correo = txtCorreo.Text;
                    clientes.Celular = (txtCelular.Text);
                    var Store = TableOperation.Insert(clientes);
                    await Coleccion.ExecuteAsync(Store);
                    Toast.MakeText(this, "¡Gracias! En breve nos comunicamos", ToastLength.Long).Show();

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();

                }
            };
            

        }

    }


    public class Clientes : TableEntity
    {
        public Clientes(string Cliente, string Nombre)
        {
            PartitionKey = Cliente;
            RowKey = Nombre;
        }
        public string Correo { get; set; }
        public string Celular { get; set; }
    }
}