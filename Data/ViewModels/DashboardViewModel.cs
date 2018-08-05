using System.Collections.Generic;
using JPFData.DTO;
using JPFData.Managers;

namespace JPFData.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            Init();
        }


        public DashboardDTO Entity { get; set; }
        public DashboardDTO SearchEntity { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public string Mode { get; set; }
        public bool IsValid { get; set; }
        public string EventCommand { get; set; }
        public string EventArgument { get; set; }

        public bool IsDetailAreaVisible { get; set; }
        public bool IsCreateAreaVisible { get; set; }
        public bool IsListAreaVisible { get; set; }
        public bool IsSearchAreaVisible { get; set; }



        private void Init()
        {
            EventCommand = "List";
            EventArgument = string.Empty;
            ValidationErrors = new List<KeyValuePair<string, string>>();

            ListMode();
        }

        private void ListMode()
        {
            IsValid = true;
            IsDetailAreaVisible = false;
            IsListAreaVisible = true;
            IsSearchAreaVisible = true;

            Mode = "List";
        }

        private void AddMode()
        {
            IsDetailAreaVisible = true;
            IsListAreaVisible = false;
            IsSearchAreaVisible = false;

            Mode = "Add";
        }

        private void Add()
        {
            IsValid = true;

            // Initialize Entity Object
            Entity = new DashboardDTO();

            // Put ViewModel into Add Mode
            AddMode();
        }

        private void EditMode()
        {
            IsDetailAreaVisible = true;
            IsListAreaVisible = false;
            IsSearchAreaVisible = false;

            Mode = "Edit";
        }

        private void Edit()
        {
            // Get Product Data

            // Put View Model into Edit Mode
            EditMode();
        }

        private void Save()
        {

            if (Mode == "Add")
            {
                //mgr.Insert(Entity);
            }
            else
            {
                //mgr.Update(Entity);
            }

            // Set any validation errors
            //ValidationErrors = mgr.ValidationErrors;
            if (ValidationErrors.Count > 0)
            {
                IsValid = false;
            }

            if (!IsValid)
            {
                if (Mode == "Add")
                {
                    AddMode();
                }
                else
                {
                    EditMode();
                }
            }
        }

        private void Delete()
        {

            // Create new entity
            Entity = new DashboardDTO();

            // Get primary key from EventArgument

            // Call data layer to delete record
            //mgr.Delete(Entity);

            // Reload the Data
            Get();

            // Set back to normal mode
            ListMode();
        }

        private void Get()
        {
            //TrainingProductManager mgr = new TrainingProductManager();

            //Products = mgr.Get(SearchEntity);

            DashboardManager mgr = new DashboardManager();

            Entity = mgr.Get(SearchEntity);
        }

        public void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "list":
                case "search":
                    Get();
                    break;

                case "add":
                    Add();
                    break;

                case "edit":
                    IsValid = true;
                    Edit();
                    break;

                case "delete":
                    //ResetSearch();
                    Delete();
                    break;

                case "save":
                    Save();
                    Get();
                    break;

                case "cancel":
                    ListMode();
                    Get();
                    break;

                case "resetsearch":
                    //ResetSearch();
                    Get();
                    break;
            }
        }
    }
}