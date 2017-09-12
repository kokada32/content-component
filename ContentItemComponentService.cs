using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Models.Domain;
using Sabio.Data;
using Sabio.Models.Requests;

namespace Sabio.Services
{
    public class ContentItemComponentService : IContentItemComponentService
    {
        readonly IDataProvider dataProvider;
        readonly Logger logger;
        public ContentItemComponentService(IDataProvider dataProvider, Logger logger)
        {
            this.dataProvider = dataProvider;
            this.logger = logger;
        }

        public ContentItemComponent GetById(int id)
        {
            var timer = logger.StartTimer("dbo.ContentItems_SelectByContentId, dbo.Subjects_SelectByContentId");
            ContentItemComponent singleItem = null;

            dataProvider.ExecuteCmd("dbo.ContentItems_SelectByContentId"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", id);
               }
               , singleRecordMapper: delegate (IDataReader reader, short set)
               {
                   singleItem = new ContentItemComponent();
                   int startingIndex = 0;

                   singleItem.Id = reader.GetSafeInt32(startingIndex++);
                   singleItem.Contents = reader.GetSafeString(startingIndex++);
                   singleItem.ContentTitle = reader.GetSafeString(startingIndex++);
                   singleItem.ContentType = reader.GetSafeString(startingIndex++);
                   singleItem.UserId = reader.GetSafeInt32(startingIndex++);
                   singleItem.DateCreated = reader.GetSafeUtcDateTime(startingIndex++);

               }
               );

            dataProvider.ExecuteCmd("dbo.Subjects_SelectByContentId"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", id);
               }
               , singleRecordMapper: delegate (IDataReader reader, short set)
               {
                   Subject listItems = new Subject();
                   int startingIndex = 0;

                   listItems.Id = reader.GetSafeInt32(startingIndex++);
                   listItems.Category = reader.GetSafeString(startingIndex++);

                   if (singleItem.Subjects == null)
                   {
                       singleItem.Subjects = new List<Subject>();
                   }

                   singleItem.Subjects.Add(listItems);
               });
            timer.Finish();
            return singleItem;


        }


        public List<Subject> GetAllCategories()
        {
            List<Subject> list = null;

            dataProvider.ExecuteCmd("dbo.Subjects_SelectAll"
                , inputParamMapper: null
                , singleRecordMapper: delegate (IDataReader reader, short set)
                  {
                      Subject singleItem = new Subject();
                      int startingIndex = 0;

                      singleItem.Id = reader.GetSafeInt32(startingIndex++);
                      singleItem.Category = reader.GetSafeString(startingIndex++);

                      if (list == null)
                      {
                          list = new List<Subject>();
                      }

                      list.Add(singleItem);
                  }
                );
            return list;
        }
        

        public List<ContentItem> GetRecentContents()
        {
            List<ContentItem> list = null;

            dataProvider.ExecuteCmd("dbo.ContentItems_SelectRecentContents"
                , inputParamMapper: null
                , singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    ContentItem singleContent = new ContentItem();
                    int startingIndex = 0; //starting ordinal

                    singleContent.Id = reader.GetSafeInt32(startingIndex++);
                    singleContent.ContentTitle = reader.GetSafeString(startingIndex++);
                    singleContent.UserId = reader.GetSafeInt32(startingIndex++);

                    if (list == null)
                   {
                       list = new List<ContentItem>();
                   }

                   list.Add(singleContent);

                });

            return list;
        }

        public List<ContentItem> GetContentsBySubjectId(int subjectId, getContentRequest model)
        {
            List<ContentItem> list = null;

            dataProvider.ExecuteCmd("dbo.ContentSubject_SelectBySubjectId"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@SubjectId", subjectId);
                   paramCollection.AddWithValue("@ItemNum", model.ItemNum);
                   paramCollection.AddWithValue("@PageNum", model.PageNum);
               }
               , singleRecordMapper: delegate (IDataReader reader, short set)
               {
                   ContentItem singleContent = new ContentItem();
                   int startingIndex = 0; //starting ordinal

                   singleContent.Id = reader.GetSafeInt32(startingIndex++);
                   singleContent.ContentTitle = reader.GetSafeString(startingIndex++);
                   singleContent.UserId = reader.GetSafeInt32(startingIndex++);

                   if (list == null)
                   {
                       list = new List<ContentItem>();
                   }

                   list.Add(singleContent);

               });

            return list;
        }


        public List<ContentItem> GetAllContents(getContentRequest model)
        {
            List<ContentItem> list = null;

            dataProvider.ExecuteCmd("dbo.ContentItems_SelectAllContents"
               , inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@itemNum", model.ItemNum);
                   paramCollection.AddWithValue("@pageNum", model.PageNum);
               }
               , singleRecordMapper: delegate (IDataReader reader, short set)
               {
                   ContentItem singleContent = new ContentItem();
                   int startingIndex = 0; //starting ordinal

                   singleContent.Id = reader.GetSafeInt32(startingIndex++);
                   singleContent.ContentTitle = reader.GetSafeString(startingIndex++);
                   singleContent.UserId = reader.GetSafeInt32(startingIndex++);

                   if (list == null)
                   {
                       list = new List<ContentItem>();
                   }

                   list.Add(singleContent);

               });

            return list;
        }
    }
}
