using AutoMapper;
using Newtonsoft.Json;
using PharmacyWebAPI.Models;
using PharmacyWebAPI.Models.Dto;
using PharmacyWebAPI.Utility.Services.IServices;
using System.Net;

namespace PharmacyWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Category = await _unitOfWork.Category.GetFirstOrDefaultAsync(p => p.Id == id);
            if (Category is null)
                return NotFound(new { success = false, message = "Not Found" });
            var obj = _mapper.Map<CategoryDto>(Category);

            return Ok(new { Category = obj });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> obj = await _unitOfWork.Category.GetAllAsync();
            return Ok(new { categories = obj });
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return Ok(new { Category = new Category() });
        }

        //POST
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { State = ModelState, Category = model });
            }
            await _unitOfWork.Category.AddAsync(model);
            await _unitOfWork.SaveAsync();

            return Ok(new { success = true, message = "Category Created Successfully", Category = model });
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Category = await _unitOfWork.Category.GetFirstOrDefaultAsync(p => p.Id == id);
            if (Category == null)
                return NotFound(new { success = false, message = "NotFound" });

            _unitOfWork.Category.Delete(Category);
            await _unitOfWork.SaveAsync();

            return Ok(new { success = true, message = "Category Deleted Successfully" });
        }

        //POST
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Category.GetFirstOrDefaultAsync(p => p.Id == id);
            if (category == null)
                return NotFound();

            return Ok(new { Category = category });
        }

        //POST
        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(Category obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { State = ModelState, Category = obj });
            }

            _unitOfWork.Category.Update(obj);
            await _unitOfWork.SaveAsync();
            return Ok(new { success = true, message = "Category Updated Successfully", Category = obj });
        }

        [HttpGet]
        [Route("GetDrugs/{id}")]
        public async Task<IActionResult> GetDrugs(int id)
        {
            var drugs = await _unitOfWork.Drug.GetAllFilterAsync(x => x.CategoryId == id, c => c.Category, z => z.Manufacturer);
            if (!drugs.Any())
                return NotFound(new { success = false, message = "Not Found" });
            var DrugsDto = _mapper.Map<IEnumerable<DrugDetailsGetDto>>(drugs);
            return Ok(new { Drugs = DrugsDto });
        }

        [HttpPost]
        [Route("AddPhoto/{id}")]
        public async Task<IActionResult> AddPhoto(int id, IFormFile file)
        {
            var category = await _unitOfWork.Category.GetFirstOrDefaultAsync(x => x.Id == id);
            if (category is null)
                return NotFound();
            if (!string.IsNullOrEmpty(category.ImageId))
            {
                var DeleteResult = await _photoService.DeletePhotoAsync(category.ImageId);
                if (DeleteResult.Error is not null)
                    return BadRequest(DeleteResult.Error);
            }
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error);

            category.ImageId = result.PublicId;
            category.ImageURL = result.Url.ToString();
            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveAsync();

            return Ok(category.ImageURL);
        }

        /*   [HttpPost]
           [Route("range")]
           public async Task<IActionResult> range()
           {
               List<Category> json = new List<Category>()
                {
       new Category{
              Name = "Pain Relief",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1681512764/da-net7/envjbh1fgfuqaubiey2b.png",
         ImageId= "da-net7/envjbh1fgfuqaubiey2b"
       },
       new Category{
              Name = "Cardiovascular Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960692/da-net7/goeulqkn8z9bdisnryiy.png",
         ImageId= "da-net7/goeulqkn8z9bdisnryiy"
       },
       new Category{
              Name = "Mental Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960705/da-net7/vgyf9fpn4yznhtihnhds.png",
         ImageId= "da-net7/vgyf9fpn4yznhtihnhds"
       },
       new Category{
              Name = "Antibiotics",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1681515617/da-net7/hedsf3misaudahcjagve.png",
         ImageId= "da-net7/hedsf3misaudahcjagve"
       },
       new Category{
              Name = "Digestive Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960717/da-net7/oijd82nehjwp47bv7s3p.png",
         ImageId= "da-net7/oijd82nehjwp47bv7s3p"
       },
       new Category{
              Name = "Allergy / Sinus Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960729/da-net7/t4s8mwmbekc03pu9r595.png",
         ImageId= "da-net7/t4s8mwmbekc03pu9r595"
       },
       new Category{
              Name = "Endocrine / Hormone Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960784/da-net7/xpptdpfvguslyv9c0r33.png",
         ImageId= "da-net7/xpptdpfvguslyv9c0r33"
       },
       new Category{
              Name = "Respiratory Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960798/da-net7/oxlvuggptau5rxl4q1ds.png",
         ImageId= "da-net7/oxlvuggptau5rxl4q1ds"
       },
       new Category{
              Name = "Dermatological Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960810/da-net7/k7l6rs6oaliku1zgvtef.png",
         ImageId= "da-net7/k7l6rs6oaliku1zgvtef"
       },
       new Category{
               Name = "Women's Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960824/da-net7/dkyf1ha1z94ifewfb2vv.png",
         ImageId= "da-net7/dkyf1ha1z94ifewfb2vv"
       },
       new Category{
               Name = "Men's Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960833/da-net7/yqsf44zcb2qgkefogoiz.png",
         ImageId= "da-net7/yqsf44zcb2qgkefogoiz"
       },
       new Category{
               Name = "Oncology",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960844/da-net7/jfvznth944kstrmwv3yt.png",
         ImageId= "da-net7/jfvznth944kstrmwv3yt"
       },
       new Category{
               Name = "Pediatrics",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960853/da-net7/sz1zgbptcmdt71coxmcf.png",
         ImageId= "da-net7/sz1zgbptcmdt71coxmcf"
       },
       new Category{
               Name = "Immunology",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960861/da-net7/nbranqjvyjvy5ll7pyyx.png",
         ImageId= "da-net7/nbranqjvyjvy5ll7pyyx"
       },
       new Category{
               Name = "Musculoskeletal Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960869/da-net7/ey0uo7cecx2qfbtrhhdn.png",
         ImageId= "da-net7/ey0uo7cecx2qfbtrhhdn"
       },
       new Category{
               Name = "Addiction Treatment",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1681513467/da-net7/e39b5xjzao5ialndpkjy.png",
         ImageId= "da-net7/e39b5xjzao5ialndpkjy"
       },
       new Category{
               Name = "Ophthalmological Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960887/da-net7/illqiu1xxq1pchw1mwqj.png",
         ImageId= "da-net7/illqiu1xxq1pchw1mwqj"
       },
       new Category{
               Name = "Neurological Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1681515080/da-net7/njgxuneb3oppmbxvycfq.png",
         ImageId= "da-net7/njgxuneb3oppmbxvycfq"
       },
       new Category{
               Name = "Gastrointestinal Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960896/da-net7/aeqgqwcdfl5hdreupoca.png",
         ImageId= "da-net7/aeqgqwcdfl5hdreupoca"
       },
       new Category{
               Name = "Reproductive Health",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960909/da-net7/eqkehqhlgqjro8yvzqhh.png",
         ImageId= "da-net7/eqkehqhlgqjro8yvzqhh"
       },
       new Category{
               Name = "Cholesterol-Lowering Medication",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1681516019/da-net7/frtesbvct4vkp4g9k51f.png",
         ImageId= "da-net7/frtesbvct4vkp4g9k51f"
       },
       new Category{
               Name = "Antidiabetic Medication",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960928/da-net7/ttwvzfdyfou8zh1xobg0.png",
         ImageId= "da-net7/ttwvzfdyfou8zh1xobg0"
       },
       new Category{
               Name = "Thyroid Hormone Replacement Medication",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960949/da-net7/cubw2bjljpjnw6oqkjx4.png",
         ImageId= "da-net7/cubw2bjljpjnw6oqkjx4"
       },
       new Category{
               Name = "Proton Pump Inhibitor (PPI)",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960958/da-net7/zaizxh7ma1kuedzoasz1.png",
         ImageId= "da-net7/zaizxh7ma1kuedzoasz1"
       },
       new Category{
               Name = "Bronchodilator",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960967/da-net7/gi2uyqlfwk42y4kqp962.png",
         ImageId= "da-net7/gi2uyqlfwk42y4kqp962"
       },
       new Category{
               Name = "Analgesic and Antipyretic Medication",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680960976/da-net7/bogfq5nb2e290ueevv6e.png",
         ImageId= "da-net7/bogfq5nb2e290ueevv6e"
       },
       new Category{
               Name = "Selective Serotonin Reuptake Inhibitor (SSRI) Antidepressant",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680961657/da-net7/gvmeuljklibdtmffnmas.png",
         ImageId= "da-net7/gvmeuljklibdtmffnmas"
       },
       new Category{
               Name = "Anticoagulant Medication",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1681518591/da-net7/isjdbfcutzo5rpcxoeq0.png",
         ImageId= "da-net7/isjdbfcutzo5rpcxoeq0"
       },
       new Category{
               Name = "Nonsteroidal Anti-Inflammatory Drug (NSAID)",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1681517578/da-net7/brkzdkzqgzss9mglr2id.png",
         ImageId= "da-net7/brkzdkzqgzss9mglr2id"
       },
       new Category{
               Name = "Allergy Relief",
         ImageURL= "http://res.cloudinary.com/dx3xe3xxp/image/upload/v1680961164/da-net7/hstaxiik1ijwniclncot.png",
         ImageId= "da-net7/hstaxiik1ijwniclncot"
       }
                };
               await _unitOfWork.Category.AddRangeAsync(json);
               await _unitOfWork.SaveAsync();
               return Ok(new { success = true, message = "Drug Created Successfully", Drug = json });
           }
       */
    }
}