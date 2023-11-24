using CommunityToolkit.Maui;
using GymSpotter.Converters;
using GymSpotter.MockDB;
using GymSpotter.Services;
using GymSpotter.ViewModels;
using GymSpotter.Views;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace GymSpotter {

    public static class MauiProgram {

        public static MauiApp CreateMauiApp() {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "IconFontTypes");
                })
                .UseMauiMaps();

            // Services
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IGymService, GymService>();
            builder.Services.AddSingleton<IReviewService, ReviewService>();
            builder.Services.AddSingleton<IFavouriteService, FavouriteService>();

            // Views
            builder.Services.AddTransient<SignUpPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<MapPage>();
#if ANDROID
            builder.Services.AddSingleton<ListViewPage>();
            builder.Services.AddSingleton<UserDetailsPage>();
            builder.Services.AddSingleton<FavouriteListViewPage>();
            builder.Services.AddSingleton<GymDetailsPage>();
#elif IOS
            builder.Services.AddSingleton<ListViewPageOnIOS>();
            builder.Services.AddSingleton<UserDetailsPageOnIOS>();
            builder.Services.AddSingleton<FavouriteListViewPageOnIOS>();
            builder.Services.AddSingleton<GymDetailsPageOnIOS>();
            
#endif

            // ViewModels
            builder.Services.AddSingleton<SignUpPageViewModel>();
            builder.Services.AddSingleton<LoginPageViewModel>();
            builder.Services.AddSingleton<UserDetailsPageViewModel>();
            builder.Services.AddSingleton<ListViewPageViewModel>();
            builder.Services.AddSingleton<GymDetailsPageViewModel>();

            // MockDataBases
            builder.Services.AddSingleton<MockUserDB>();
            builder.Services.AddSingleton<MockGymDB>();
            builder.Services.AddSingleton<MockReviewDB>();
            builder.Services.AddSingleton<MockFavouriteDB>();
#if ANDROID
            builder.Services.AddTransient<EditProfilePage>();
            builder.Services.AddTransient<EditGymPage>();
            builder.Services.AddTransient<AddReviewPage>();
#elif IOS
            builder.Services.AddTransient<EditGymPageOnIOS>();
            builder.Services.AddTransient<EditProfilePageOnIOS>();
            builder.Services.AddTransient<AddReviewPage>();
#endif
#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Removing underline from entry
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) => {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
            });

            // Removing underline from editor
            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(Editor), (handler, view) => {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
            });

            return builder.Build();
        }
    }
}