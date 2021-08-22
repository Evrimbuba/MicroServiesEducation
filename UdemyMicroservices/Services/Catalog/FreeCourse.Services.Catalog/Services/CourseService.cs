using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Services
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CourseService(IMapper mapper,IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(p => true).ToListAsync();
            courses = await IncludeHelper(courses);
            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var course = _mapper.Map<Course>(courseCreateDto);
            course.CreationDate = DateTime.Now;
            await _courseCollection.InsertOneAsync(course);
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 201);
        }

        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var course = _mapper.Map<Course>(courseUpdateDto);
            var updateCourse = await _courseCollection.FindOneAndReplaceAsync(x => x.CourseId == course.CourseId,course);
            if (updateCourse == null)
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }
            return Response<NoContent>.Success(204);
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(x => x.CourseId == id);
            if (result.DeletedCount > 0)
            {
                return Response<NoContent>.Success(200);
            }
            return Response<NoContent>.Fail("Course not found", 404);
        }

        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            // Filter işlemine örnek
            //FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(p => p.Name, name);

            var course = await _courseCollection.Find<Course>(x => x.CourseId == id).FirstOrDefaultAsync();
            if (course == null)
            {
                return Response<CourseDto>.Fail("Course not found", 404);
            }
            course.Category = await _categoryCollection.Find<Category>(x => x.CategoryId == course.CategoryId).FirstAsync();
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }

        public async Task<Response<List<CourseDto>>> GetByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();
            courses = await IncludeHelper(courses);
            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        private async Task<List<Course>> IncludeHelper(List<Course> courses)
        {
            //if (courses.Any())
            //{
            //    foreach (var item in courses)
            //    {
            //        item.Category = await _categoryCollection.Find<Category>(x => x.CategoryId == item.CategoryId).FirstOrDefaultAsync();
            //    }
            //    return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
            //}
            //return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(new List<Course>()), 200);
            if (courses.Any())
            {
                foreach (var item in courses)
                {
                    item.Category = await _categoryCollection.Find<Category>(x => x.CategoryId == item.CategoryId).FirstOrDefaultAsync();
                }
                return courses;
            }
            return new List<Course>();
        }
    }
}
