using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Sassie2
{
    internal class HondaCPOInspectionReport
    {
        private string _assignmentID;
        private string _divisionCode;
        private DataSet _dsCPOData;
        private readonly SassieApi _sassieApi;
        private List<Dictionary<int, string>> _presale_list = new List<Dictionary<int, string>>();
        private List<Dictionary<int, string>> _postsale_list = new List<Dictionary<int, string>>();
        private Dictionary<string, Dictionary<string, string>> _presale_vehicles = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, Dictionary<string, string>> _postsale_vehicles = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> _inspection_data = new Dictionary<string, string>();

        public HondaCPOInspectionReport()
        {
            _sassieApi = new SassieApi();

            _presale_list.Add(QuestionMapping.presale_mappingA);
            _presale_list.Add(QuestionMapping.presale_mappingB);
            _presale_list.Add(QuestionMapping.presale_mappingC);
            _presale_list.Add(QuestionMapping.presale_mappingD);
            _presale_list.Add(QuestionMapping.presale_mappingE);
            _presale_list.Add(QuestionMapping.presale_mappingF);
            _presale_list.Add(QuestionMapping.presale_mappingG);
            _presale_list.Add(QuestionMapping.presale_mappingH);
            _presale_list.Add(QuestionMapping.presale_mappingI);
            _presale_list.Add(QuestionMapping.presale_mappingJ);

            _postsale_list.Add(QuestionMapping.postsale_mappingA);
            _postsale_list.Add(QuestionMapping.postsale_mappingB);
            _postsale_list.Add(QuestionMapping.postsale_mappingC);
            _postsale_list.Add(QuestionMapping.postsale_mappingD);
            _postsale_list.Add(QuestionMapping.postsale_mappingE);
            _postsale_list.Add(QuestionMapping.postsale_mappingF);
            _postsale_list.Add(QuestionMapping.postsale_mappingG);
            _postsale_list.Add(QuestionMapping.postsale_mappingH);
            _postsale_list.Add(QuestionMapping.postsale_mappingI);
            _postsale_list.Add(QuestionMapping.postsale_mappingJ);
        }

        public async void GetData()
        {
            try
            {
                _assignmentID = "26228303";
                _assignmentID = "23183043";
                _assignmentID = "22790360";
                _assignmentID = "26224623";//8 post-sale, no pre-sale 
                _assignmentID = "26224953";//acura 
                _assignmentID = "26224446";//with comments

                _dsCPOData = new DBHondaCPO().GetHondaCPOOCR(Convert.ToInt32(_assignmentID), "en");

                _divisionCode = _dsCPOData.Tables[0].Rows[0]["Division_Code"].ToString().Trim();
                var surveyID = _divisionCode == "B" ? "1061" : "1039";
                var clientLocationID = _dsCPOData.Tables[0].Rows[0]["Dealer_Code"].ToString().Trim();

                //string test = JsonConvert.SerializeObject(_dsCPOData.Tables[3]);

                //_strImagePath = _dvCPOData[0].Table.Rows[0]["PDF_File_Name"].ToString().Trim().Remove(0, 11);
                //_NoPostSaleVehicles = Convert.ToInt32(_dvCPOData[0].Table.Rows[0]["OVehInspected"]);
                //_NoPreSaleVehicles = Convert.ToInt32(_dvCPOData[0].Table.Rows[0]["RVehInspected"]);

                //1. Consultation information 
                //2. Dealer information 
                //3. Dealer contact information 
                //4. Inspection summary 
                //5. Vehicle compliance findings 
                //6. Post-sale (Documentation inspection only)
                //7. Pre-sale (Documentation and Vehicle inspection)
                //8. Facility inspection 
                //9. Facility images 

                _inspection_data = new Dictionary<string, string>() {
                    {"survey_id", surveyID },
                    {"client_location_id", clientLocationID }
                };

                ConsultationInformation();
                DealerInformation();
                PopulateVehicles();
                PopulatePostsaleQuestions();
                PopulatePresaleQuestions();
                FacilityInspection();

                var client_data = new
                {
                    grant_type = "client_credentials",
                    client_id = "WSwDiUqqv5Q2InctWBHkWeTWmDmfiNJl",
                    client_secret = "62UEIr61r2FQc9xyvRn4PBdmRQ4gTPwa"
                };
                string client_json = JsonConvert.SerializeObject(client_data);
                //string token = await sassieApi.AuthenticateAsync(client_json);

                string inspection_json = JsonConvert.SerializeObject(_inspection_data);
                // await sassieApi.PostDataAsync(inspection_json, token);


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                _dsCPOData = null;
            }
        }

        private void ConsultationInformation()
        {

            foreach (var item in QuestionMapping.consultation_mapping)
            {
                _inspection_data.Add(item.Value, _dsCPOData.Tables[0].Rows[0][item.Key].ToString());
            }

            _inspection_data.Add("question_1", Convert.ToDateTime(_dsCPOData.Tables[0].Rows[0]["Audit_Date"]).ToString("yyyy-MM-dd"));
            _inspection_data.Add("question_21", Convert.ToDateTime(_dsCPOData.Tables[0].Rows[0]["Audit_Date"]).ToShortTimeString());

        }

        private void DealerInformation()
        {
            foreach (var item in QuestionMapping.dealer_mapping)
            {
                _inspection_data.Add(item.Value, _dsCPOData.Tables[0].Rows[0][item.Key].ToString());
            }

        }

        private void PopulateVehicles()
        {
            string vin;
            Dictionary<string, string> detail;
            foreach (DataRow item in _dsCPOData.Tables[1].Rows)
            {
                vin = item["Vehicle_VIN"].ToString();
                detail = new Dictionary<string, string>()
                    {
                         {"VIN",item["Vehicle_VIN"].ToString() },
                         {"Manufacturer","" },
                        {"Make_Description",item["Make_Description"].ToString() },
                        {"Model_Description", item["Model_Description"].ToString()},
                        {"Vehicle_Year", item["Vehicle_Year"].ToString() },
                        {"Stock_Number", item["Stock_ID"].ToString() },
                        {"Tier", item["Tier_Data"].ToString() },
                    };

                if (item["Audit_Type"].Equals("Pre"))
                {
                    _presale_vehicles.Add(vin, detail);
                }
                else
                {
                    _postsale_vehicles.Add(vin, detail);
                }
            }
        }

        private void PopulatePostsaleQuestions()
        {
            string vin_num;
            Dictionary<int, string> q_mapping;
            int qid;
            int ind = 0;
            string value;
            foreach (var pair in _postsale_vehicles)
            {
                vin_num = pair.Key;
                q_mapping = _postsale_list[ind];

                _inspection_data.Add(q_mapping[ind], "Yes");

                foreach (var item in QuestionMapping.vehicle_detail)
                {
                    if (!q_mapping.ContainsKey(item.Key))
                    {
                        continue;
                    }

                    _inspection_data.Add(q_mapping[item.Key], pair.Value[item.Value].Trim());
                }

                foreach (DataRow row in _dsCPOData.Tables[3].Rows)
                {
                    qid = (int)row["Question_ID"];
                    value = row[vin_num].ToString().Trim();
                    if (!q_mapping.ContainsKey(qid))
                    {
                        continue;
                    }

                    _inspection_data.Add(q_mapping[qid], value);

                    if (!value.ToLower().Equals("yes"))
                    {
                        if (!QuestionMapping.comments_mapping.ContainsKey(q_mapping[qid]))
                        {
                            throw new Exception(string.Format("comments question missing for {0}!!", q_mapping[qid]));
                        }

                        _inspection_data.Add(QuestionMapping.comments_mapping[q_mapping[qid]], "comments_dummy_test");
                    }

                }
                ind++;
            }

        }

        private void PopulatePresaleQuestions()
        {
            string vin_num;
            Dictionary<int, string> q_mapping;
            int qid;
            int ind = 0;
            string value;
            foreach (var pair in _presale_vehicles)
            {
                vin_num = pair.Key;

                q_mapping = _presale_list[ind];

                _inspection_data.Add(q_mapping[ind], "Yes");

                foreach (var item in QuestionMapping.vehicle_detail)
                {
                    if (!q_mapping.ContainsKey(item.Key))
                    {
                        continue;
                    }

                    _inspection_data.Add(q_mapping[item.Key], pair.Value[item.Value].Trim());
                }

                foreach (DataRow row in _dsCPOData.Tables[5].Rows)
                {
                    qid = (int)row["Question_ID"];
                    value = row[vin_num].ToString().Trim();
                    if (!q_mapping.ContainsKey(qid))
                    {
                        continue;
                    }

                    _inspection_data.Add(q_mapping[qid], value);

                    if (!value.ToLower().Equals("yes"))
                    {
                        if (!QuestionMapping.comments_mapping.ContainsKey(q_mapping[qid]))
                        {
                            throw new Exception(string.Format("comments question missing for {0}!!", q_mapping[qid]));
                        }

                        _inspection_data.Add(QuestionMapping.comments_mapping[q_mapping[qid]], "comments_dummy_test");
                    }
                }
                ind++;
            }

        }

        private void FacilityInspection()
        {
            int qid;
            int qval;
            foreach (DataRow row in _dsCPOData.Tables[6].Rows)
            {
                qid = (int)row["Question_ID"];
                qval = (int)row["Question_Value"];
                if (!QuestionMapping.facility_mapping.ContainsKey(qid))
                {
                    continue;
                }
                _inspection_data.Add(QuestionMapping.facility_mapping[qid], QuestionMapping.objective[qval].Trim());
            }
        }
    }
}
