using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Sassie2
{
    internal class HondaCPOInspectionReport
    {
        string _assignmentID;
        string _divisionCode;
        DataSet _dsCPOData;
        SassieApi sassieApi;

        List<Dictionary<int, string>> presale_list = new List<Dictionary<int, string>>();
        List<Dictionary<int, string>> postsale_list = new List<Dictionary<int, string>>();

        Dictionary<string, Dictionary<string, string>> presale_vehicles = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, Dictionary<string, string>> postsale_vehicles = new Dictionary<string, Dictionary<string, string>>();

        Dictionary<string, string> inspection_data = new Dictionary<string, string>();

        public HondaCPOInspectionReport()
        {
            sassieApi = new SassieApi();

            presale_list.Add(QuestionMapping.presale_mappingA);
            presale_list.Add(QuestionMapping.presale_mappingB);
            presale_list.Add(QuestionMapping.presale_mappingC);
            presale_list.Add(QuestionMapping.presale_mappingD);
            presale_list.Add(QuestionMapping.presale_mappingE);
            presale_list.Add(QuestionMapping.presale_mappingF);
            presale_list.Add(QuestionMapping.presale_mappingG);
            presale_list.Add(QuestionMapping.presale_mappingH);
            presale_list.Add(QuestionMapping.presale_mappingI);
            presale_list.Add(QuestionMapping.presale_mappingJ);

            postsale_list.Add(QuestionMapping.postsale_mappingA);
            postsale_list.Add(QuestionMapping.postsale_mappingB);
            postsale_list.Add(QuestionMapping.postsale_mappingC);
            postsale_list.Add(QuestionMapping.postsale_mappingD);
            postsale_list.Add(QuestionMapping.postsale_mappingE);
            postsale_list.Add(QuestionMapping.postsale_mappingF);
            postsale_list.Add(QuestionMapping.postsale_mappingG);
            postsale_list.Add(QuestionMapping.postsale_mappingH);
            postsale_list.Add(QuestionMapping.postsale_mappingI);
            postsale_list.Add(QuestionMapping.postsale_mappingJ);
        }

        public async void GetData()
        {
            try
            {
                string surveyID = "";
                //_assignmentID = "26228303";
                //_assignmentID = "23183043";
                _assignmentID = "22790360";
                _assignmentID = "26224623";//8 post-sale, no pre-sale 
                _assignmentID = "26224953";//acura 

                _dsCPOData = new DBHondaCPO().GetHondaCPOOCR(Convert.ToInt32(_assignmentID), "en");

                _divisionCode = _dsCPOData.Tables[0].Rows[0]["Division_Code"].ToString().Trim();
                surveyID = _divisionCode == "B" ? "1061" : "1039";

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

                inspection_data = new Dictionary<string, string>() {
                    {"survey_id", surveyID },
                    {"client_location_id", "1001" }
                };

                PopulateVehicles();
                ConsultationInformation();
                DealerInformation();
                FacilityInspection();
                PopulatePresaleQuestions();
                PopulatePostsaleQuestions();

                var client_data = new
                {
                    grant_type = "client_credentials",
                    client_id = "WSwDiUqqv5Q2InctWBHkWeTWmDmfiNJl",
                    client_secret = "62UEIr61r2FQc9xyvRn4PBdmRQ4gTPwa"
                };
                string client_json = JsonConvert.SerializeObject(client_data);
                //string token = await sassieApi.AuthenticateAsync(client_json);

                string inspection_json = JsonConvert.SerializeObject(inspection_data);
                // await sassieApi.PostDataAsync(inspection_json, token);


                ////Vehicle Compliance Information 	
                //if (_dvCPOData[1].Table.Rows.Count > 0)
                //{
                //    // CreateVehicleComplianceInfo();
                //}
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

        private void PopulatePresaleQuestions()
        {
            string vin_num;
            Dictionary<int, string> q_mapping;
            int qid;
            int ind = 0;
            foreach (var pair in presale_vehicles)
            {
                vin_num = pair.Key;

                q_mapping = presale_list[ind];

                inspection_data.Add(q_mapping[ind], "Yes");

                foreach (var item in QuestionMapping.vehicle_detail)
                {
                    if (!q_mapping.ContainsKey(item.Key))
                    {
                        continue;
                    }

                    inspection_data.Add(q_mapping[item.Key], pair.Value[item.Value].Trim());
                }

                foreach (DataRow row in _dsCPOData.Tables[5].Rows)
                {
                    qid = (int)row["Question_ID"];
                    if (!q_mapping.ContainsKey(qid))
                    {
                        continue;
                    }

                    if (row[vin_num].ToString().ToLower().Equals("no"))
                    {
                        throw new Exception("comments question required!!");
                    }
                    inspection_data.Add(q_mapping[qid], row[vin_num].ToString().Trim());
                }
                ind++;
            }

        }

        private void PopulatePostsaleQuestions()
        {
            string vin_num;
            Dictionary<int, string> q_mapping;
            int qid;
            int ind = 0;
            foreach (var pair in postsale_vehicles)
            {
                vin_num = pair.Key;
                q_mapping = postsale_list[ind];

                inspection_data.Add(q_mapping[ind], "Yes");

                foreach (var item in QuestionMapping.vehicle_detail)
                {
                    if (!q_mapping.ContainsKey(item.Key))
                    {
                        continue;
                    }

                    inspection_data.Add(q_mapping[item.Key], pair.Value[item.Value].Trim());
                }

                foreach (DataRow row in _dsCPOData.Tables[3].Rows)
                {
                    qid = (int)row["Question_ID"];
                    if (!q_mapping.ContainsKey(qid))
                    {
                        continue;
                    }
                    if (row[vin_num].ToString().ToLower().Equals("no"))
                    {
                        throw new Exception("comments question required!!");
                    }
                    inspection_data.Add(q_mapping[qid], row[vin_num].ToString().Trim());
                }
                ind++;
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
                    presale_vehicles.Add(vin, detail);
                }
                else
                {
                    postsale_vehicles.Add(vin, detail);
                }
            }
        }

        private void FacilityInspection()
        {
            ////question_141
            ////_dvCPOData[6].Table.Rows[1]["Question_Value"]
            //result_dict.Add("question_141", _dvCPOData[6].Table.Rows[1]["Question_Value"].ToString());
            ////question_161
            ////_dvCPOData[6].Table.Rows[2]["Question_Value"]
            //result_dict.Add("question_161", _dvCPOData[6].Table.Rows[2]["Question_Value"].ToString());
            ////question_181
            ////_dvCPOData[6].Table.Rows[3]["Question_Value"]
            //result_dict.Add("question_181", _dvCPOData[6].Table.Rows[3]["Question_Value"].ToString());
            ////question_201
            ////_dvCPOData[6].Table.Rows[4]["Question_Value"]
            //result_dict.Add("question_201", _dvCPOData[6].Table.Rows[4]["Question_Value"].ToString());

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
                inspection_data.Add(QuestionMapping.facility_mapping[qid], QuestionMapping.objective[qval].Trim());
            }
        }

        private void DealerInformation()
        {
            foreach (var item in QuestionMapping.dealer_mapping)
            {
                inspection_data.Add(item.Value, _dsCPOData.Tables[0].Rows[0][item.Key].ToString());
            }

            ////question_31
            ////_dvCPOData[0][0]["Dealer_Contact1"]
            //result_dict.Add("question_31", _dvCPOData[0][0]["Dealer_Contact1"].ToString());
            ////question_41
            ////_dvCPOData[0][0]["Email_Address1"]
            //result_dict.Add("question_41", _dvCPOData[0][0]["Email_Address1"].ToString());
            ////question_51
            ////_dvCPOData[0][0]["Dealer_Contact2"]
            //result_dict.Add("question_51", _dvCPOData[0][0]["Dealer_Contact2"].ToString());
            ////question_61
            ////_dvCPOData[0][0]["Email_Address2"]
            //result_dict.Add("question_61", _dvCPOData[0][0]["Email_Address2"].ToString());
            ////question_71
            ////_dvCPOData[0][0]["Dealer_Contact3"]
            //result_dict.Add("question_71", _dvCPOData[0][0]["Dealer_Contact3"].ToString());
            ////question_81
            ////_dvCPOData[0][0]["Email_Address3"]
            //result_dict.Add("question_81", _dvCPOData[0][0]["Email_Address3"].ToString());
            ////question_91
            ////_dvCPOData[0][0]["Dealer_Contact4"]
            //result_dict.Add("question_91", _dvCPOData[0][0]["Dealer_Contact4"].ToString());
            ////question_101
            ////_dvCPOData[0][0]["Email_Address4"]
            //result_dict.Add("question_101", _dvCPOData[0][0]["Email_Address4"].ToString());
        }

        private void ConsultationInformation()
        {

            foreach (var item in QuestionMapping.consultation_mapping)
            {
                inspection_data.Add(item.Value, _dsCPOData.Tables[0].Rows[0][item.Key].ToString());
            }

            inspection_data.Add("question_1", Convert.ToDateTime(_dsCPOData.Tables[0].Rows[0]["Audit_Date"]).ToString("yyyy-MM-dd"));
            inspection_data.Add("question_21", Convert.ToDateTime(_dsCPOData.Tables[0].Rows[0]["Audit_Date"]).ToShortTimeString());

            ////question_11
            ////_dvCPOData[0][0]["Assignment_ID"]
            //result_dict.Add("question_11", (_dvCPOData[0][0]["Assignment_ID"].ToString()));
            ////question_2741
            ////_dvCPOData[0][0]["Inspector_ID"]
            //result_dict.Add("question_2741", _dvCPOData[0][0]["Inspector_ID"].ToString());
            ////question_1
            ////Convert.ToDateTime(_dvCPOData[0][0]["Audit_Date"]).ToShortDateString()
            //result_dict.Add("question_1", Convert.ToDateTime(_dvCPOData[0][0]["Audit_Date"]).ToShortDateString());
            ////question_21
            ////Convert.ToDateTime(_dvCPOData[0][0]["Audit_Date"]).ToShortTimeString()
            //result_dict.Add("question_21", Convert.ToDateTime(_dvCPOData[0][0]["Audit_Date"]).ToShortTimeString());
        }
    }
}
