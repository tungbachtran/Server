﻿namespace api.Model.DtoModels
{
    public class CreateCourseDto
    {
        public string CourseId { get; set; }
        public string Name { get; set; }
        public double Credits { get; set; }
        public bool isAvailable { get; set; }
        public string? requiredCourseId { get; set; }
    }
}
