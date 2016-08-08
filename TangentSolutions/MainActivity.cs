using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using RestSharp;
using System.Collections.Generic;

namespace TangentSolutions
{
	[Activity(Label = "TangentSolutions", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			EditText txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
			EditText txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
			Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);

			Utils utils = new Utils();
			utils.pref = Application.Context.GetSharedPreferences("Session Data", FileCreationMode.Private);
			utils.edit = utils.pref.Edit();

			txtUsername.Text = "admin";
			txtPassword.Text = "admin";

			btnLogin.Click += (sender, e) =>
			{
				if (string.IsNullOrEmpty(txtUsername.Text))
				{
					txtUsername.SetError("Required field", null);
					txtUsername.RequestFocus();
					return;
				}

				if (string.IsNullOrEmpty(txtPassword.Text))
				{
					txtPassword.SetError("Required field", null);
					txtPassword.RequestFocus();
					return;
				}

				utils.edit.PutString("Username", txtUsername.Text);
				utils.edit.PutString("Password", txtPassword.Text).Commit();

				new doThreadLogin(this).Execute();
			};
		}

		public void DisplayError(Context context, string errorMessage)
		{
			AlertDialog alert = new AlertDialog.Builder(context).Create();
			alert.SetCancelable(false);
			alert.SetTitle("TANGENT SOLUTIONS");
			alert.SetMessage(errorMessage);
			alert.SetButton("OK", (sender, e) =>
				{
					alert.Dismiss();
				});
			alert.Show();
		}

		private class doThreadLogin : AsyncTask<String, String, String>
		{
			Context context;
			ProgressDialog pdIntro = null;
			protected MainActivity activity;
			List<Project> projects = new List<Project>();
			public doThreadLogin(Context context)
			{
				this.context = context;
				this.activity = new MainActivity();
			}

			protected override void OnPreExecute()
			{
				base.OnPreExecute();

				pdIntro = new ProgressDialog(context); //ProgressDialog.Show(this, "Processing","Please Wait...",false, true);
				pdIntro.SetCancelable(false);
				pdIntro.Indeterminate = true;
				pdIntro.SetMessage("Please Wait...");
				pdIntro.SetProgressStyle(ProgressDialogStyle.Spinner);
				pdIntro.SetTitle("Processing");
				pdIntro.Show();

			}

			protected void PublishProgress(params Java.Lang.Object[] values)
			{
				base.PublishProgress(values);
			}

			protected override void OnProgressUpdate(params Java.Lang.Object[] values)
			{
				pdIntro.SetMessage(values[0].ToString());
			}

			protected override void OnPostExecute(Java.Lang.Object @params)
			{
				base.OnPostExecute(@params);

				Utils utils = new Utils();
				utils.pref = context.GetSharedPreferences("Session Data", FileCreationMode.Private);
				utils.edit = utils.pref.Edit();

				pdIntro.Hide();
				pdIntro.Dismiss();
				pdIntro.Dispose();

				if (utils.pref.GetBoolean("HasError", false) == true)
				{
					activity.DisplayError(context, utils.pref.GetString("ErrorMessage", ""));
					utils.edit.PutBoolean("HasError", false);
					utils.edit.PutString("ErrorMessage", "");
					utils.edit.Commit();
				}
				else
				{
					Intent projectList = new Intent(context, typeof(MenuActivity));
					var MySerializedObject = JsonConvert.SerializeObject(projects);
					projectList.PutExtra("MyProjects", MySerializedObject);
					context.StartActivity(projectList);
					activity.Finish();
				}
			}

			protected override string RunInBackground(params string[] @params)
			{
				Utils utils = new Utils();
				utils.pref = context.GetSharedPreferences("Session Data", FileCreationMode.Private);
				utils.edit = utils.pref.Edit();

				try
				{
					Person person = new Person();
					person.Username = "admin";
					person.Password = "admin";

					var client = new RestClient("http://userservice.staging.tangentmicroservices.com/api-token-auth/");
					var request = new RestRequest(Method.POST);
					request.AddHeader("postman-token", "58b4fc0e-885d-ebee-ce86-ed828ce9552b");
					request.AddHeader("cache-control", "no-cache");
					request.AddHeader("authorization", "Basic YWRtaW46YWRtaW4=");
					request.AddHeader("content-type", "multipart/form-data; boundary=---011000010111000001101001");
					request.AddParameter("multipart/form-data; boundary=---011000010111000001101001", "-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"username\"\r\n\r\n" + person.Username + "\r\n-----011000010111000001101001\r\nContent-Disposition: form-data; name=\"password\"\r\n\r\n" + person.Password + "\r\n-----011000010111000001101001--", ParameterType.RequestBody);
					IRestResponse response = client.Execute(request);

					// execute the request
					var content = response.Content; // raw content as string

					JObject jsonObject = JsonConvert.DeserializeObject<JObject>(content);
					string strToken = jsonObject["token"].Value<string>();

					client = new RestClient("http://projectservice.staging.tangentmicroservices.com:80/api/v1/projects/");
					request = new RestRequest(Method.GET);
					request.AddHeader("postman-token", "03985fd2-3574-3e63-71a5-db33d20edaa2");
					request.AddHeader("cache-control", "no-cache");
					request.AddHeader("authorization", "Token " + strToken);
					request.AddHeader("content-type", "application/json");
					response = client.Execute(request);

					JArray jsonObjects = JsonConvert.DeserializeObject<JArray>(response.Content);

					foreach (JObject obj in jsonObjects)
					{
						var _jsonProject = JObject.Parse(obj.ToString());
						Project _project = new Project() { Description = _jsonProject["description"].ToString(), EndDate = Convert.ToDateTime(_jsonProject["end_date"].ToString()), Title = _jsonProject["title"].ToString(), IsActive = Convert.ToBoolean(_jsonProject["is_active"].ToString()), IsBillable = Convert.ToBoolean(_jsonProject["is_billable"].ToString()), PK = Convert.ToInt32(_jsonProject["pk"].ToString()), Resources = null, StartDate = Convert.ToDateTime(_jsonProject["start_date"].ToString()), Tasks = null };
						projects.Add(_project);
					}

					utils.edit.PutString("Token", strToken);
					utils.edit.PutBoolean("HasError", false).Commit();
				}
				catch (Exception ex)
				{
					utils.edit.PutBoolean("HasError", true);
					utils.edit.PutString("ErrorMessage", ex.Message).Commit();
				}


				return "";
			}
		}
	}
}


