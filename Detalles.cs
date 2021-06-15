using Android.App;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Widget;
using AndroidX.Core.Graphics.Drawable;
using System;

namespace Agencia_de_Viajes__PF_PDM_ITSN601_
{
    [Activity(Label = "Detalles")]
    public class Detalles : Activity
    {
        TextView txtPais, txtCosto, txtCiudades, txtDuracion, txtAerolinea, txtDescripcion;
        ImageView Imagena, Imagenb, Imagenc;
        Button Interesa;
        string pais, costo, imagena, imagenb, imagenc, ciudades, duracion, aerolinea, descripcion;       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Detalle);

            var imgaero = FindViewById<ImageView>(Resource.Id.imgaero);
            var imgcd = FindViewById<ImageView>(Resource.Id.imgcd);
            var imgcal = FindViewById<ImageView>(Resource.Id.imgduracion);
            var imgcosto = FindViewById<ImageView>(Resource.Id.imgcosto);
            var imgmundo = FindViewById<ImageView>(Resource.Id.imgmundo);

            imgaero.SetImageResource(Resource.Drawable.avion);
            imgcd.SetImageResource(Resource.Drawable.ciudad);
            imgcal.SetImageResource(Resource.Drawable.calendario);
            imgcosto.SetImageResource(Resource.Drawable.costo);
            imgmundo.SetImageResource(Resource.Drawable.mundo);


            try
            {
             
                pais = Intent.GetStringExtra("nombre");
                costo = Intent.GetStringExtra("costo");
                imagena = Intent.GetStringExtra("imagena");
                imagenb = Intent.GetStringExtra("imagenb");
                imagenc = Intent.GetStringExtra("imagenc");
                ciudades = Intent.GetStringExtra("ciudades");
                duracion = Intent.GetStringExtra("duracion");
                aerolinea = Intent.GetStringExtra("aerolinea");
                descripcion = Intent.GetStringExtra("descripcion");

                Interesa = FindViewById<Button>(Resource.Id.btnsiguiente);
                Imagena = FindViewById<ImageView>(Resource.Id.imga);
                Imagenb = FindViewById<ImageView>(Resource.Id.imgb);
                Imagenc = FindViewById<ImageView>(Resource.Id.imgc);
                txtPais = FindViewById<TextView>(Resource.Id.txtpais);
                txtCosto = FindViewById<TextView>(Resource.Id.txtcosto);
                txtCiudades = FindViewById<TextView>(Resource.Id.txtdestinos);
                txtDuracion = FindViewById<TextView>(Resource.Id.txtduracion);
                txtAerolinea = FindViewById<TextView>(Resource.Id.txtaerolinea);
                txtDescripcion = FindViewById<TextView>(Resource.Id.txtdescripcion);

                txtPais.Text = pais;
                txtCosto.Text = costo;
                txtCiudades.Text = ciudades;
                txtDuracion.Text = duracion;
                txtAerolinea.Text = aerolinea;
                txtDescripcion.Text = descripcion;
                

                var RutaImagena = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),imagena);
                var RutaImagenb = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),imagenb);
                var RutaImagenc = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),imagenc);
                var rutauriimagena = Android.Net.Uri.Parse(RutaImagena);
                var rutauriimagenb = Android.Net.Uri.Parse(RutaImagenb);
                var rutauriimagenc = Android.Net.Uri.Parse(RutaImagenc);

                Imagena.SetImageURI(rutauriimagena);
                Imagenb.SetImageURI(rutauriimagenb);
                Imagenc.SetImageURI(rutauriimagenc);

                var opciones = new BitmapFactory.Options();
                opciones.InPreferredConfig = Bitmap.Config.Argb8888;

                var bitmapa = BitmapFactory.DecodeFile(RutaImagena, opciones);
                var bitmapb = BitmapFactory.DecodeFile(RutaImagenb, opciones);
                var bitmapc = BitmapFactory.DecodeFile(RutaImagenc, opciones);

                Imagena.SetImageDrawable(getRoundedCornerImage(bitmapa, 20));
                Imagenb.SetImageDrawable(getRoundedCornerImage(bitmapb, 20));
                Imagenc.SetImageDrawable(getRoundedCornerImage(bitmapc, 20));

            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }

            Interesa.Click += delegate
            {
                try
                {
                    var DataIntent = new Intent(this, typeof(C_Clientes));
                    DataIntent.PutExtra("nombre", pais);
                    Toast.MakeText(this, "¡Ya casi es tuyo! ", ToastLength.Long).Show();
                    StartActivity(DataIntent);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();

                }
            };

        }
       
        public static RoundedBitmapDrawable
           getRoundedCornerImage(Bitmap image, int cornerRadius)
        {
            var corner = RoundedBitmapDrawableFactory.Create(null, image);
            corner.CornerRadius = cornerRadius;
            return corner;
        }


    }
}