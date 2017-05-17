
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
            loadItems = ViewModel.ExecuteLoadItemsCommand();


            MessagingCenter.Subscribe<AddItemActivity, Item>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Item;
                await ViewModel.AddItem(_item);
            });
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
            var item = ViewModel.Items[e.Position];
            var intent = new Intent(Activity, typeof(BrowseItemDetailActivity));

            intent.PutExtra("data", Newtonsoft.Json.JsonConvert.SerializeObject(item));
            Activity.StartActivity(intent);
        }
        
        public void BecameVisible()
        {
        }
    }

    class BrowseItemsAdapter : BaseRecycleViewAdapter
    {

        ItemsViewModel viewModel;
        Activity activity;

        public BrowseItemsAdapter(Activity activity, ItemsViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.activity = activity;

            this.viewModel.Items.CollectionChanged += (sender, args) =>
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
            var item = viewModel.Items[position];

            // Replace the contents of the view with that element
            var myHolder = holder as MyViewHolder;
            myHolder.TextView.Text = item.Text;
            myHolder.DetailTextView.Text = item.Description;
        }

        public override int ItemCount => viewModel.Items.Count;


    }

    public class MyViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; set; }
        public TextView DetailTextView { get; set; }

        public MyViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
                            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Android.Resource.Id.Text1);
            DetailTextView = itemView.FindViewById<TextView>(Android.Resource.Id.Text2);
            itemView.Click += (sender, e) => clickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RecyclerClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

}

