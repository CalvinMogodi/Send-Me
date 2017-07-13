
using System;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SendMe.ViewModel;
using Android.Support.V4.Widget;
using Android.App;
using Android.Content;
using SendMe.Helpers;
using SendMe.Services;
using System.Threading.Tasks;
using SendMe.Model;
using SendMe.Droid.Helpers;
using SendMe.ViewModels;
using Android.Graphics;

namespace SendMe.Droid
{
    public class BrowseFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {

        public static BrowseFragment NewInstance() =>
            new BrowseFragment { Arguments = new Bundle() };

        BrowseItemsAdapter adapter;
        Task loadItems;

        ProgressBar progress;
        public ItemsViewModel ViewModel
        {
            get;
            set;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ServiceLocator.Instance.Register<MockDataStore, MockDataStore>();

            ViewModel = new ItemsViewModel();
            loadItems = ViewModel.GetAdvertsAsync();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_browse, container, false);
            return view;
        }


        public override void OnStart()
        {
            base.OnStart();
        }


        public override void OnStop()
        {
            base.OnStop();         
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MessagingCenter.Unsubscribe<AddItemActivity>(this, "AddItem");
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            //var item = ViewModel.Items[e.Position];
            //var intent = new Intent(Activity, typeof(BrowseItemDetailActivity));

            //intent.PutExtra("data", Newtonsoft.Json.JsonConvert.SerializeObject(item));
            //Activity.StartActivity(intent);
        }
        
        public void BecameVisible()
        {
        }
    }

    class BrowseItemsAdapter : BaseRecycleViewAdapter
    {

        QuotesViewModel viewModel;
        Activity activity;

        public BrowseItemsAdapter(Activity activity, QuotesViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.activity = activity;

            this.viewModel.Quotes.CollectionChanged += (sender, args) =>
            {
                this.activity.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.item_browse;
            itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new MyViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var quote = viewModel.Quotes[position];
            Context mContext = Android.App.Application.Context;
            ImageManager imageManager = new ImageManager(mContext);

            // Replace the contents of the view with that element
            var myHolder = holder as MyViewHolder;
            myHolder.TextView.Text = quote.CourierName;
            myHolder.DetailTextView.Text = String.Format("{0} - ({1})", quote.Price, quote.CourierKmDistance);
            //myHolder.ProfilePictureImageView.SetImageBitmap(imageManager.ConvertStringToBitMap(quote.CourierProfilePicture));
            Bitmap bMap = BitmapFactory.DecodeResource(mContext.Resources, Resource.Drawable.profile_generic);
            myHolder.ProfilePictureImageView.SetImageBitmap(bMap);
        }

        public override int ItemCount => viewModel.Quotes.Count;


    }

    public class MyViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; set; }
        public TextView DetailTextView { get; set; }

        public ImageView ProfilePictureImageView { get; set; }

        public MyViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Android.Resource.Id.Text1);
            DetailTextView = itemView.FindViewById<TextView>(Android.Resource.Id.Text2);
            ProfilePictureImageView = itemView.FindViewById<ImageView>(Resource.Id.imageView1);
            itemView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }
}

