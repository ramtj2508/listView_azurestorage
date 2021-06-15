using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.AppCompat.App;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.IO;
using System.Linq;
using Android.Content;
using System.Threading.Tasks;
using Android.Graphics;

namespace Agencia_de_Viajes__PF_PDM_ITSN601_
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        //Variables Recyclerview
        RecyclerView RecicladordeVista;
        RecyclerView.LayoutManager AdministradorInterfaz;
        PhotoAlbumAdapter Adaptador;
        AlbumFotos mAlbumdeFotos;

        //Variables Lista 
        Android.App.ProgressDialog progress;
        string elementoimagena, elementoimagenb, elementoimagenc;
        ListView listado;
        List<Country> ListadodePaises = new List<Country>();
        List<ElementosdelaTabla> ElementosTabla = new List<ElementosdelaTabla>();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);        
            SetContentView(Resource.Layout.activity_main); 
            SupportActionBar.Hide();

            var img1 = FindViewById<ImageView>(Resource.Id.img1);
            img1.SetImageResource(Resource.Drawable.logo);

            mAlbumdeFotos = new AlbumFotos();
            RecicladordeVista = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            AdministradorInterfaz = new LinearLayoutManager(this);
            RecicladordeVista.SetLayoutManager(AdministradorInterfaz);
            Adaptador = new PhotoAlbumAdapter(mAlbumdeFotos);
            Adaptador.SeleccionElemento += OnItemClick;
            RecicladordeVista.SetAdapter(Adaptador);
            
            listado = FindViewById<ListView>(Resource.Id.listview);
            progress = new Android.App.ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            progress.SetMessage("Cargando datos de Azure ...");
            progress.SetCancelable(false);
            progress.Show();
            await CargarDatosAzure();
            progress.Hide();

        }

        public async Task CargarDatosAzure()
        {
            try
            {


                //Transferir Blob y datos TablaNoSql
                var CuentadeAlmacenamiento = CloudStorageAccount.Parse
                     ("DefaultEndpointsProtocol=https;AccountName=programacionparamovil;AccountKey=1MHCQg8Cmym2unWhGkWu3SY0ctyppH8vfnYNOy3lRjUAzA9TVJnHycMnduZtvb7dacoN4/rJ02B0dg2qP520UQ==;EndpointSuffix=core.windows.net");
                var ClienteBlob = CuentadeAlmacenamiento.CreateCloudBlobClient();
                var Contenedor = ClienteBlob.GetContainerReference("agencia");
                var TablaNoSQL = CuentadeAlmacenamiento.CreateCloudTableClient();
                var Tabla = TablaNoSQL.GetTableReference("agencia");
                var Consulta = new TableQuery<Country>();
                TableContinuationToken token = null;
                var Datos = await Tabla.ExecuteQuerySegmentedAsync<Country>
                    (Consulta, token, null, null);
                ListadodePaises.AddRange(Datos.Results);
                int iNombre = 0;
                int iCosto = 0;
                int iImagena = 0;
                int iImagenb = 0;
                int iImagenc = 0;
                int iCiudades = 0;
                int iDuracion = 0;
                int iAerolinea = 0;
                int iDescripcion = 0;

                ElementosTabla = ListadodePaises.Select(r => new ElementosdelaTabla()
                {
                    Nombre = ListadodePaises.ElementAt(iNombre++).RowKey,
                    Costo = ListadodePaises.ElementAt(iCosto++).Costo,
                    Imagena = ListadodePaises.ElementAt(iImagena++).IMGa,
                    Imagenb = ListadodePaises.ElementAt(iImagenb++).IMGb,
                    Imagenc = ListadodePaises.ElementAt(iImagenc++).IMGc,
                    Ciudades = ListadodePaises.ElementAt(iCiudades++).Ciudades,
                    Duracion = ListadodePaises.ElementAt(iDuracion++).Duracion,
                    Aerolinea = ListadodePaises.ElementAt(iAerolinea++).Aerolienea,
                    Descripcion = ListadodePaises.ElementAt(iDescripcion++).Descripcion,

                }).ToList();

                int contadorimagen = 0;
                while (contadorimagen < ListadodePaises.Count)
                {
                    elementoimagena = ListadodePaises.ElementAt(contadorimagen).IMGa;
                    elementoimagenb = ListadodePaises.ElementAt(contadorimagen).IMGb;
                    elementoimagenc = ListadodePaises.ElementAt(contadorimagen).IMGc;

                    var ImagenBloba = Contenedor.GetBlockBlobReference(elementoimagena);
                    var ImagenBlobb = Contenedor.GetBlockBlobReference(elementoimagenb);
                    var ImagenBlobc = Contenedor.GetBlockBlobReference(elementoimagenc);

                    var rutaimagena = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    var rutaimagenb = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    var rutaimagenc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                    var ArchivoImagena = System.IO.Path.Combine(rutaimagena, elementoimagena);
                    var ArchivoImagenb = System.IO.Path.Combine(rutaimagena, elementoimagenb);
                    var ArchivoImagenc = System.IO.Path.Combine(rutaimagena, elementoimagenc);

                    var StreamImagena = File.OpenWrite(ArchivoImagena);
                    var StreamImagenb = File.OpenWrite(ArchivoImagenb);
                    var StreamImagenc = File.OpenWrite(ArchivoImagenc);

                    await ImagenBloba.DownloadToStreamAsync(StreamImagena);
                    await ImagenBlobb.DownloadToStreamAsync(StreamImagenb);
                    await ImagenBlobc.DownloadToStreamAsync(StreamImagenc);

                    contadorimagen++;
                }

                Toast.MakeText(this, "Imágenes descargadas", ToastLength.Long).Show();
                listado.Adapter = new AdaptadorDatos(this, ElementosTabla);
                listado.ItemClick += OnListItemClick;
            }

            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }

        }

        public void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var DataSend = ElementosTabla[e.Position];
            var DataIntent = new Intent(this, typeof(Detalles));

            DataIntent.PutExtra("nombre", DataSend.Nombre);
            DataIntent.PutExtra("costo", DataSend.Costo);
            DataIntent.PutExtra("imagena", DataSend.Imagena);
            DataIntent.PutExtra("imagenb", DataSend.Imagenb);
            DataIntent.PutExtra("imagenc", DataSend.Imagenc);
            DataIntent.PutExtra("ciudades", DataSend.Ciudades);
            DataIntent.PutExtra("duracion", DataSend.Duracion);
            DataIntent.PutExtra("aerolinea", DataSend.Aerolinea);
            DataIntent.PutExtra("descripcion", DataSend.Descripcion);


            StartActivity(DataIntent);
        }

        void OnItemClick(object sender, int posicion)
        {
            int NumerodeFoto = posicion+1;
            if (NumerodeFoto == 1) Toast.MakeText(this, "Trabaja cómodo", ToastLength.Short).Show();
            if (NumerodeFoto == 2) Toast.MakeText(this, "Su mejor opción", ToastLength.Short).Show();
            if (NumerodeFoto == 3) Toast.MakeText(this, "Viaja tranquilo", ToastLength.Short).Show();
            if (NumerodeFoto == 4) Toast.MakeText(this, "Los mejores hoteles", ToastLength.Short).Show();
            if (NumerodeFoto == 5) Toast.MakeText(this, "Siénte en casa", ToastLength.Short).Show();
            if (NumerodeFoto == 6) Toast.MakeText(this, "Disfruta, conecta", ToastLength.Short).Show();
            if (NumerodeFoto == 7) Toast.MakeText(this, "Inolvidable", ToastLength.Short).Show();
            if (NumerodeFoto == 8) Toast.MakeText(this, "Moméntos únicos", ToastLength.Short).Show();
        }
    }


    public class PhotoViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Imagen { get; private set; }
        public TextView Texto { get; private set; }
        public PhotoViewHolder(View Elemento, Action<int> Colocador)
            : base(Elemento)
        {
            Imagen = Elemento.FindViewById<ImageView>(Resource.Id.imagen);
            Texto = Elemento.FindViewById<TextView>(Resource.Id.texto);
            Elemento.Click += (sender, e) => Colocador(base.LayoutPosition);
        }
    }
    public class PhotoAlbumAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> SeleccionElemento;
        public AlbumFotos mAlbumFotos;
        public PhotoAlbumAdapter(AlbumFotos AlbumFotos)
        {
            mAlbumFotos = AlbumFotos;
        }
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup Contenido, int viewType)
        {
            View VistadeElemento = LayoutInflater.From(Contenido.Context).
                        Inflate(Resource.Layout.PhotoCard, Contenido, false);
            PhotoViewHolder pvh = new PhotoViewHolder(VistadeElemento, OnClick);
            return pvh;
        }
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder cabecera, int posicion)
        {
            PhotoViewHolder pvh = cabecera as PhotoViewHolder;
            pvh.Imagen.SetImageResource(mAlbumFotos[posicion].FotoID);
            pvh.Texto.Text = mAlbumFotos[posicion].TextoFoto;
        }
        public override int ItemCount
        {
            get { return mAlbumFotos.CantidaddeFotos; }
        }
        void OnClick(int position)
        {
            SeleccionElemento(this, position);
        }
    }
    public class Fotografias
    {
        public int mFotoID;
        public string mTextoFoto;
        public int FotoID { get { return mFotoID; } }
        public string TextoFoto { get { return mTextoFoto; } }
    }
    public class AlbumFotos
    {
        static Fotografias[] ConjuntodeFotos = {
            new Fotografias { mFotoID = Resource.Drawable.uno,
                         },
            new Fotografias { mFotoID = Resource.Drawable.dos,
                         },
            new Fotografias { mFotoID = Resource.Drawable.tres,
                         },
            new Fotografias { mFotoID = Resource.Drawable.cuatro,
                         },
            new Fotografias { mFotoID = Resource.Drawable.cinco,
                        },
            new Fotografias { mFotoID = Resource.Drawable.seis,
                         },
            new Fotografias { mFotoID = Resource.Drawable.siete,
                         },
            new Fotografias { mFotoID = Resource.Drawable.ocho,
                         },
            };
        private Fotografias[] Fotos;
        public AlbumFotos()
        {
            Fotos = ConjuntodeFotos;
        }
        public int CantidaddeFotos
        {
            get { return Fotos.Length; }
        }
        public Fotografias this[int i]
        {
            get { return Fotos[i]; }
        }
    }
   
    public class ElementosdelaTabla
    {
        public string Nombre { get; set; }
        public string Costo { get; set; }
        public string Imagena { get; set; }
        public string Imagenb { get; set; }
        public string Imagenc { get; set; }
        public string Ciudades { get; set; }
        public string Duracion { get; set; }
        public string Aerolinea { get; set; }
        public string Descripcion { get; set; }

    }
    public class Country : TableEntity
    {

        public Country(string Paises, string Nombre)
        {
            PartitionKey = Paises;
            RowKey = Nombre;
        }
        public Country() { }
        public string Costo { get; set; }
        public string IMGa { get; set; }
        public string IMGb { get; set; }
        public string IMGc { get; set; }
        public string Ciudades { get; set; }
        public string Duracion { get; set; }
        public string Aerolienea { get; set; }
        public string Descripcion { get; set; }
    }







}