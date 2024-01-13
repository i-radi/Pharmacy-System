using AutoMapper;
using PharmacyWebAPI.Models;
using PharmacyWebAPI.Models.Dto;
using PharmacyWebAPI.Models.Enums;
using PharmacyWebAPI.Utility.Services.IServices;

namespace PharmacyWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DrugController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public DrugController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var drug = await _unitOfWork.Drug.GetFirstOrDefaultAsync(p => p.Id == id, c => c.Category, d => d.Manufacturer);
            if (drug is null)
                return NotFound(new { success = false, message = "Not Found" });
            var obj = _mapper.Map<DrugDetailsGetDto>(drug);

            return Ok(new { Drug = obj });
        }

        [HttpGet]
        [Route("fixPrice")]
        public async Task<IActionResult> fixPrice()
        {
            var drugs = await _unitOfWork.Drug.GetAllAsync();
            foreach (var drug in drugs)
                drug.Price = (int)drug.Price;

            _unitOfWork.Drug.UpdateRange(drugs);
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpGet]
        [Route("fixStock")]
        public async Task<IActionResult> fixStock()
        {
            var drugs = await _unitOfWork.Drug.GetAllAsync();
            foreach (var drug in drugs)
            {
                if (drug.Stock <= 1)
                {
                    drug.Stock += 10;
                }
            }

            _unitOfWork.Drug.UpdateRange(drugs);
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Drug> Drug = await _unitOfWork.Drug.GetAllAsync(c => c.Category, z => z.Manufacturer);
            var DrugDetailsGetDto = _mapper.Map<IEnumerable<DrugDetailsGetDto>>(Drug);
            return Ok(new { Drugs = DrugDetailsGetDto });
        }

        //POST
        [HttpPost]
        [Route("QuickCreate")]
        public async Task<IActionResult> QuickCreate()
        {
            var model = new Drug
            {
                CategoryId = (await _unitOfWork.Category.GetFirstOrDefaultAsync()).Id,
                ManufacturerId = (await _unitOfWork.Manufacturer.GetFirstOrDefaultAsync()).Id
            };
            await _unitOfWork.Drug.AddAsync(model);
            await _unitOfWork.SaveAsync();
            return Ok(new { success = true, message = "Drug Created Successfully", model });
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            PostDrugDto PostDrugDto = new()
            {
                Categories = await _unitOfWork.Category.GetAllAsync(),
                Manufacturers = await _unitOfWork.Manufacturer.GetAllAsync()
            };
            return Ok(new { Drug = PostDrugDto });
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(PostDrugDto obj)
        {
            if (!ModelState.IsValid)
            {
                obj.Categories = await _unitOfWork.Category.GetAllAsync();
                obj.Manufacturers = await _unitOfWork.Manufacturer.GetAllAsync();
                return BadRequest(new { State = ModelState, Drug = obj });
            }

            var drug = _mapper.Map<Drug>(obj);

            await _unitOfWork.Drug.AddAsync(drug);
            await _unitOfWork.SaveAsync();
            return Ok(new { success = true, message = "Drug Created Successfully", Drug = drug });
        }

        //POST
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _unitOfWork.Drug.GetFirstOrDefaultAsync(p => p.Id == id);
            if (obj == null)
            {
                return NotFound(new { success = false, message = "Not Found" });
            }
            _unitOfWork.Drug.Delete(obj);
            await _unitOfWork.SaveAsync();
            return Ok(new { success = true, message = "Drug Deleted Successfully" });
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var drug = await _unitOfWork.Drug.GetFirstOrDefaultAsync(p => p.Id == id);
            if (drug == null)
                return NotFound(new { success = false, message = "Not Found" });

            var drugDto = _mapper.Map<PostDrugDto>(drug);
            drugDto.Categories = await _unitOfWork.Category.GetAllAsync();
            drugDto.Manufacturers = await _unitOfWork.Manufacturer.GetAllAsync();

            return Ok(new { Drug = drugDto });
        }

        //Put
        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(PostDrugDto obj)
        {
            if (!ModelState.IsValid)
            {
                obj.Categories = await _unitOfWork.Category.GetAllAsync();
                obj.Manufacturers = await _unitOfWork.Manufacturer.GetAllAsync();
                return BadRequest(new { State = ModelState, Drug = obj }); ;
            }

            Drug drug = _mapper.Map<Drug>(obj);

            _unitOfWork.Drug.Update(drug);
            await _unitOfWork.SaveAsync();
            return Ok(new { success = true, message = "Drug Updated Successfully", Drug = drug });
        }

        [HttpPost]
        [Route("AddPhoto/{id}")]
        public async Task<IActionResult> AddPhoto(int id, IFormFile file)
        {
            var drug = await _unitOfWork.Drug.GetFirstOrDefaultAsync(x => x.Id == id);
            if (drug == null)
                return NotFound();
            if (!string.IsNullOrEmpty(drug.ImageId))
            {
                var DeleteResult = await _photoService.DeletePhotoAsync(drug.ImageId);
                if (DeleteResult.Error is not null)
                    return BadRequest(DeleteResult.Error);
            }
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error);
            drug.ImageId = result.PublicId;
            drug.ImageURL = result.SecureUrl.AbsoluteUri;
            _unitOfWork.Drug.Update(drug);
            await _unitOfWork.SaveAsync();

            return Ok(drug.ImageURL);
        }

        /*    [HttpPost]
            [Route("range")]
            public async Task<IActionResult> range()
            {
                List<Drug> drugs = new List<Drug>()
                {
        new Drug{
                Name= "Amoxicillin",
          DosageForm = DosageForm.Capsule,
          DosageStrength= "500mg",
          SideEffects = "nausea, vomiting, diarrhea, rash",
          Contraindications= "allergy to penicillin or cephalosporins",
          Description= "Amoxicillin is an antibiotic used to treat a wide range of bacterial infections.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 16,
          Price= 94.51,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974563/da-net7/mhyesce14xvpj97wiwno.png",
          ImageId= "da-net7/mhyesce14xvpj97wiwno",
          ManufacturerId= 16,
          CategoryId = 4,
        },
        new Drug{
                Name= "Lisinopril",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "10mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "history of angioedema, pregnancy",
          Description= "Lisinopril is an angiotensin-converting enzyme (ACE) inhibitor used to treat high blood pressure and heart failure.",
          PregnancyCategory = PregnancyCategory.D,
          Stock= 20,
          Price= 11.37,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974574/da-net7/x5esj2i6xttuz1j9msh2.png",
          ImageId= "da-net7/x5esj2i6xttuz1j9msh2",
          ManufacturerId= 21,
          CategoryId = 2,
        },
        new Drug{
                Name= "Simvastatin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "20mg",
          SideEffects = "muscle pain, weakness, headache",
          Contraindications= "pregnancy, liver disease",
          Description= "Simvastatin is a statin cholesterol-lowering medication used to reduce the risk of heart disease and stroke.",
          PregnancyCategory = PregnancyCategory.X,
          Stock= 20,
          Price= 81.29,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974583/da-net7/b1y8hpuncjuxhlzfyt1p.png",
          ImageId= "da-net7/b1y8hpuncjuxhlzfyt1p",
          ManufacturerId= 20,
          CategoryId = 21,
        },
        new Drug{
                Name= "Metformin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "nausea, diarrhea, stomach upset",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "Metformin is an antidiabetic medication used to treat type 2 diabetes.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 60.44,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974593/da-net7/g3vcybookfsymlrl42fc.png",
          ImageId= "da-net7/g3vcybookfsymlrl42fc",
          ManufacturerId= 19,
          CategoryId = 22,
        },
        new Drug{
                Name= "Levothyroxine",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "50mcg",
          SideEffects = "weight loss, tremors, headache",
          Contraindications= "heart disease, thyrotoxicosis, adrenal gland problems",
          Description= "Levothyroxine is a thyroid hormone replacement medication used to treat hypothyroidism.",
          PregnancyCategory = PregnancyCategory.A,
          Stock= 20,
          Price= 63.27,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974602/da-net7/mzaiedp27vwkyafzocs2.png",
          ImageId= "da-net7/mzaiedp27vwkyafzocs2",
          ManufacturerId= 18,
          CategoryId = 23,
        },
        new Drug{
                Name= "Omeprazole",
          DosageForm = DosageForm.Capsule,
          DosageStrength= "20mg",
          SideEffects = "headache, diarrhea, stomach pain",
          Contraindications= "allergy to proton pump inhibitors, liver disease",
          Description= "Omeprazole is a proton pump inhibitor (PPI) used to treat gastroesophageal reflux disease (GERD) and other acid-related conditions.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 14,
          Price= 18.52,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974612/da-net7/kzaqjcxfkhcpeenuznck.png",
          ImageId= "da-net7/kzaqjcxfkhcpeenuznck",
          ManufacturerId= 17,
          CategoryId = 24,
        },
        new Drug{
                Name= "Aspirin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "325mg",
          SideEffects = "stomach upset, heartburn, bleeding",
          Contraindications= "allergy to aspirin, bleeding disorders, pregnancy",
          Description= "Aspirin is a nonsteroidal anti-inflammatory drug (NSAID) used to treat pain, fever, and inflammation. It is also used to prevent blood clots and reduce the risk of heart attack and stroke.",
          PregnancyCategory = PregnancyCategory.D,
          Stock= 20,
          Price= 20.49,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974630/da-net7/n4h7g6hh88n27lybxm0e.png",
          ImageId= "da-net7/n4h7g6hh88n27lybxm0e",
          ManufacturerId= 26,
          CategoryId = 29,
        },
        new Drug{
                Name= "Albuterol",
          DosageForm = DosageForm.Inhaler,
          DosageStrength= "90mcg per actuation",
          SideEffects = "tremors, headache, nervousness",
          Contraindications= "heart disease, high blood pressure, diabetes",
          Description= "Albuterol is a bronchodilator used to treat asthma and other breathing disorders.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 37.46,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974639/da-net7/qvlogql6cohvwioi7q05.png",
          ImageId= "da-net7/qvlogql6cohvwioi7q05",
          ManufacturerId= 25,
          CategoryId = 25,
        },
        new Drug{
                Name= "Paracetamol",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "nausea, stomach pain, liver damage",
          Contraindications= "alcoholism, liver disease",
          Description= "Paracetamol is an analgesic and antipyretic medication used to treat mild to moderate pain and fever.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 12.33,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974648/da-net7/ltuunyiktismpri8ipuo.png",
          ImageId= "da-net7/ltuunyiktismpri8ipuo",
          ManufacturerId= 24,
          CategoryId = 26,
        },
        new Drug{
                Name= "Citalopram",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "20mg",
          SideEffects = "nausea, dry mouth, drowsiness",
          Contraindications= "heart disease, history of seizures, use of MAO inhibitors",
          Description= "Citalopram is a selective serotonin reuptake inhibitor (SSRI) antidepressant used to treat depression and anxiety disorders.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 18,
          Price= 91.07,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974685/da-net7/e0bltm8xuxbhpbngiykr.png",
          ImageId= "da-net7/e0bltm8xuxbhpbngiykr",
          ManufacturerId= 23,
          CategoryId = 27,
        },
        new Drug{
                Name= "Warfarin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "5mg",
          SideEffects = "bleeding, bruising, stomach pain",
          Contraindications= "pregnancy, bleeding disorders, liver disease",
          Description= "Warfarin is an anticoagulant medication used to prevent blood clots and reduce the risk of stroke and heart attack.",
          PregnancyCategory = PregnancyCategory.X,
          Stock= 20,
          Price= 29.91,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974674/da-net7/myra0p74gl122yshpfrb.png",
          ImageId= "da-net7/myra0p74gl122yshpfrb",
          ManufacturerId= 22,
          CategoryId = 28,
        },
        new Drug{
                Name= "Acetaminophen",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "nausea, stomach pain, rash",
          Contraindications= "liver disease, alcoholism, G6PD deficiency",
          Description= "Acetaminophen (also known as paracetamol) is a pain reliever and fever reducer.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 67.82,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974698/da-net7/trfbztqykzgrjxvqd4wa.png",
          ImageId= "da-net7/trfbztqykzgrjxvqd4wa",
          ManufacturerId= 1,
          CategoryId = 1,
        },
        new Drug{
                Name= "Loratadine",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "10mg",
          SideEffects = "dry mouth, headache, drowsiness",
          Contraindications= "kidney disease, liver disease, pregnancy",
          Description= "Loratadine is an antihistamine used to treat allergies.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 7.44,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974706/da-net7/vqmmjsgthssfac2b3ihd.png",
          ImageId= "da-net7/vqmmjsgthssfac2b3ihd",
          ManufacturerId= 2,
          CategoryId = 30,
        },
        new Drug{
                Name= "Metoprolol",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "50mg",
          SideEffects = "dizziness, fatigue, depression",
          Contraindications= "heart failure, asthma, diabetes",
          Description= "Metoprolol is a beta-blocker used to treat high blood pressure and heart conditions.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 14.41,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974716/da-net7/anutd2owvkuokujign7q.png",
          ImageId= "da-net7/anutd2owvkuokujign7q",
          ManufacturerId= 3,
          CategoryId = 2,
        },
        new Drug{
                Name= "Ciprofloxacin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "nausea, diarrhea, headache",
          Contraindications= "tendon problems, myasthenia gravis, pregnancy",
          Description= "Ciprofloxacin is an antibiotic used to treat bacterial infections.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 22.09,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974729/da-net7/texxkjwdg2rplrno5p1m.png",
          ImageId= "da-net7/texxkjwdg2rplrno5p1m",
          ManufacturerId= 4,
          CategoryId = 4,
        },
        new Drug{
                Name= "Tadalafil",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "20mg",
          SideEffects = "headache, back pain, muscle aches",
          Contraindications= "heart disease, low blood pressure, stroke",
          Description= "Tadalafil is a phosphodiesterase-5 inhibitor used to treat erectile dysfunction and pulmonary arterial hypertension.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 85.7,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974738/da-net7/qqvgind1yxneeohij4e9.png",
          ImageId= "da-net7/qqvgind1yxneeohij4e9",
          ManufacturerId= 5,
          CategoryId = 11,
        },
        new Drug{
                Name= "Sertraline",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "100mg",
          SideEffects = "nausea, diarrhea, insomnia",
          Contraindications= "mania, liver disease, bipolar disorder",
          Description= "Sertraline is a selective serotonin reuptake inhibitor (SSRI) used to treat depression, anxiety, and obsessive-compulsive disorder.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 78.22,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974748/da-net7/mdlnzj38scoycoukhqdx.png",
          ImageId= "da-net7/mdlnzj38scoycoukhqdx",
          ManufacturerId= 10,
          CategoryId = 3,
        },
        new Drug{
                Name= "Hydrochlorothiazide",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "25mg",
          SideEffects = "dizziness, headache, dry mouth",
          Contraindications= "liver disease, diabetes, gout",
          Description= "Hydrochlorothiazide is a thiazide diuretic used to treat high blood pressure and fluid retention.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 31.53,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974760/da-net7/ytjobueiqedvasi4hhrl.png",
          ImageId= "da-net7/ytjobueiqedvasi4hhrl",
          ManufacturerId= 6,
          CategoryId = 2,
        },
        new Drug{
                Name= "Amoxicillin",
          DosageForm = DosageForm.Capsule,
          DosageStrength= "500mg",
          SideEffects = "nausea, vomiting, diarrhea",
          Contraindications= "mononucleosis, liver disease, asthma",
          Description= "Amoxicillin is a penicillin antibiotic used to treat bacterial infections.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 54.69,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974770/da-net7/bm2miipgqj8iseazoo8z.png",
          ImageId= "da-net7/bm2miipgqj8iseazoo8z",
          ManufacturerId= 14,
          CategoryId = 4,
        },
        new Drug{
                Name= "Rosuvastatin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "10mg",
          SideEffects = "muscle pain, diarrhea, headache",
          Contraindications= "liver disease, pregnancy, breastfeeding",
          Description= "Rosuvastatin is a statin used to lower cholesterol levels and prevent heart disease.",
          PregnancyCategory = PregnancyCategory.X,
          Stock= 20,
          Price= 13.68,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974782/da-net7/xh53xwzymatasmoczs0x.png",
          ImageId= "da-net7/xh53xwzymatasmoczs0x",
          ManufacturerId= 7,
          CategoryId = 2,
        },
        new Drug{
                Name= "Levothyroxine",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "50mcg",
          SideEffects = "hair loss, weight loss, insomnia",
          Contraindications= "thyrotoxicosis, adrenal gland problems, heart disease",
          Description= "Levothyroxine is a thyroid hormone replacement used to treat hypothyroidism and prevent goiter.",
          PregnancyCategory = PregnancyCategory.A,
          Stock= 20,
          Price= 4.46,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974791/da-net7/as2hcc2l6dzow7elbdxu.png",
          ImageId= "da-net7/as2hcc2l6dzow7elbdxu",
          ManufacturerId= 15,
          CategoryId = 7,
        },
        new Drug{
                Name= "Ibuprofen",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "200mg",
          SideEffects = "nausea, stomach pain, headache",
          Contraindications= "asthma, pregnancy, bleeding disorder",
          Description= "Ibuprofen is a nonsteroidal anti-inflammatory drug (NSAID) used to treat pain and fever.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 5.49,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974801/da-net7/tvdyvwhdoo2sj4m0pzjq.png",
          ImageId= "da-net7/tvdyvwhdoo2sj4m0pzjq",
          ManufacturerId= 8,
          CategoryId = 1,
        },
        new Drug{
                Name= "Losartan",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "50mg",
          SideEffects = "dizziness, headache, fatigue",
          Contraindications= "pregnancy, liver disease, kidney disease",
          Description= "Losartan is an angiotensin II receptor blocker (ARB) used to treat high blood pressure and protect kidneys from damage in diabetic patients.",
          PregnancyCategory = PregnancyCategory.D,
          Stock= 20,
          Price= 28.57,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974815/da-net7/hvog9rl3rkd0gjmsraan.png",
          ImageId= "da-net7/hvog9rl3rkd0gjmsraan",
          ManufacturerId= 9,
          CategoryId = 2,
        },
        new Drug{
                Name= "Alprazolam",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "0.5mg",
          SideEffects = "drowsiness, dizziness, headache",
          Contraindications= "glaucoma, liver disease, depression",
          Description= "Alprazolam is a benzodiazepine used to treat anxiety and panic disorders.",
          PregnancyCategory = PregnancyCategory.D,
          Stock= 20,
          Price= 81.39,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974824/da-net7/qbgxf0csrj2uytzoaqov.png",
          ImageId= "da-net7/qbgxf0csrj2uytzoaqov",
          ManufacturerId= 10,
          CategoryId = 3,
        },
        new Drug{
                Name= "Doxycycline",
          DosageForm = DosageForm.Capsule,
          DosageStrength= "100mg",
          SideEffects = "nausea, diarrhea, skin rash",
          Contraindications= "pregnancy, children under 8 years old, liver disease",
          Description= "Doxycycline is a tetracycline antibiotic used to treat bacterial infections.",
          PregnancyCategory = PregnancyCategory.D,
          Stock= 20,
          Price= 67.02,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974834/da-net7/zyctaowijogpehtrbwer.png",
          ImageId= "da-net7/zyctaowijogpehtrbwer",
          ManufacturerId= 11,
          CategoryId = 4,
        },
        new Drug{
                Name= "Metformin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "nausea, diarrhea, stomach upset",
          Contraindications= "kidney disease, liver disease, alcohol use",
          Description= "Metformin is an oral diabetes medicine used to control blood sugar levels.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 15.95,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974844/da-net7/nq8mahwtvldapsgzlljv.png",
          ImageId= "da-net7/nq8mahwtvldapsgzlljv",
          ManufacturerId= 15,
          CategoryId = 7,
        },
        new Drug{
                Name= "Fluoxetine",
          DosageForm = DosageForm.Capsule,
          DosageStrength= "20mg",
          SideEffects = "nausea, headache, insomnia",
          Contraindications= "bipolar disorder, liver disease, seizures",
          Description= "Fluoxetine is a selective serotonin reuptake inhibitor (SSRI) used to treat depression, anxiety, and obsessive-compulsive disorder.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 36.08,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974854/da-net7/h9av2ka2ryd3wrcuxvjo.png",
          ImageId= "da-net7/h9av2ka2ryd3wrcuxvjo",
          ManufacturerId= 14,
          CategoryId = 3,
        },
        new Drug{
                Name= "Omeprazole",
          DosageForm = DosageForm.Capsule,
          DosageStrength= "20mg",
          SideEffects = "headache, nausea, diarrhea",
          Contraindications= "liver disease, osteoporosis, low magnesium levels",
          Description= "Omeprazole is a proton pump inhibitor (PPI) used to treat heartburn and acid reflux.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 26.55,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974865/da-net7/ang8exr0y9kjyual0mey.png",
          ImageId= "da-net7/ang8exr0y9kjyual0mey",
          ManufacturerId= 7,
          CategoryId = 5,
        },
        new Drug{
                Name= "Tramadol",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "50mg",
          SideEffects = "nausea, dizziness, constipation",
          Contraindications= "severe asthma, liver disease, seizures",
          Description= "Tramadol is a narcotic-like pain reliever used to treat moderate to severe pain.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 90.25,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974876/da-net7/tgtddvvu4cpmf5tkjogq.png",
          ImageId= "da-net7/tgtddvvu4cpmf5tkjogq",
          ManufacturerId= 14,
          CategoryId = 1,
        },
        new Drug{
                Name= "Lisinopril",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "10mg",
          SideEffects = "dizziness, headache, dry cough",
          Contraindications= "pregnancy, kidney disease, liver disease",
          Description= "Lisinopril is an angiotensin-converting enzyme (ACE) inhibitor used to treat high blood pressure and heart failure.",
          PregnancyCategory = PregnancyCategory.D,
          Stock= 20,
          Price= 30.31,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974884/da-net7/ux9bzn0wmuqpttbrsozo.png",
          ImageId= "da-net7/ux9bzn0wmuqpttbrsozo",
          ManufacturerId= 15,
          CategoryId = 2,
        },
        new Drug{
                Name= "Cetirizine",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "10mg",
          SideEffects = "drowsiness, dry mouth, headache",
          Contraindications= "kidney disease, liver disease, pregnancy",
          Description= "Cetirizine is an antihistamine used to treat allergies and hives.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 63.43,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974894/da-net7/geazv3ojtwhut1r1oug0.png",
          ImageId= "da-net7/geazv3ojtwhut1r1oug0",
          ManufacturerId= 15,
          CategoryId = 6,
        },
        new Drug{
                Name= "Morphine",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "15mg",
          SideEffects = "drowsiness, constipation, nausea",
          Contraindications= "breathing problems, head injury, liver disease",
          Description= "Morphine is a narcotic pain reliever used to treat severe pain.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 22.79,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974903/da-net7/exuleiivazefjdlw8ukc.png",
          ImageId= "da-net7/exuleiivazefjdlw8ukc",
          ManufacturerId= 15,
          CategoryId = 1,
        },
        new Drug{
                Name= "Metoprolol",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "25mg",
          SideEffects = "dizziness, fatigue, depression",
          Contraindications= "heart block, asthma, liver disease",
          Description= "Metoprolol is a beta blocker used to treat high blood pressure and prevent heart attacks.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 60.86,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974914/da-net7/um74ktfibgviij7hphaz.png",
          ImageId= "da-net7/um74ktfibgviij7hphaz",
          ManufacturerId= 14,
          CategoryId = 2,
        },
        new Drug{
                Name= "Ciprofloxacin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "nausea, diarrhea, headache",
          Contraindications= "pregnancy, children under 18 years old, liver disease",
          Description= "Ciprofloxacin is a fluoroquinolone antibiotic used to treat bacterial infections.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 26.77,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974925/da-net7/qmtxktaekxyd68yjidmq.png",
          ImageId= "da-net7/qmtxktaekxyd68yjidmq",
          ManufacturerId= 14,
          CategoryId = 4,
        },
        new Drug{
                Name= "Furosemide",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "40mg",
          SideEffects = "dizziness, headache, muscle cramps",
          Contraindications= "anuria, liver disease, diabetes",
          Description= "Furosemide is a loop diuretic used to treat fluid retention and high blood pressure.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 69.37,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974938/da-net7/dry1eabountvbsdgytqu.png",
          ImageId= "da-net7/dry1eabountvbsdgytqu",
          ManufacturerId= 13,
          CategoryId = 2,
        },
        new Drug{
                Name= "Atorvastatin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "20mg",
          SideEffects = "muscle pain, diarrhea, headache",
          Contraindications= "liver disease, pregnancy, breastfeeding",
          Description= "Atorvastatin is a statin used to lower cholesterol levels and prevent heart disease.",
          PregnancyCategory = PregnancyCategory.X,
          Stock= 20,
          Price= 40.37,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974950/da-net7/l5wzb6a80qojjbf2u3os.png",
          ImageId= "da-net7/l5wzb6a80qojjbf2u3os",
          ManufacturerId= 14,
          CategoryId = 2,
        },
        new Drug{
                Name= "Sertraline",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "50mg",
          SideEffects = "nausea, diarrhea, insomnia",
          Contraindications= "bipolar disorder, liver disease, seizures",
          Description= "Sertraline is a selective serotonin reuptake inhibitor (SSRI) used to treat depression, anxiety, and obsessive-compulsive disorder.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 88.95,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974962/da-net7/egvhet1ahjlev2gc8fuf.png",
          ImageId= "da-net7/egvhet1ahjlev2gc8fuf",
          ManufacturerId= 15,
          CategoryId = 3,
        },
        new Drug{
                Name= "Lorazepam",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "1mg",
          SideEffects = "drowsiness, dizziness, confusion",
          Contraindications= "glaucoma, liver disease, respiratory depression",
          Description= "Lorazepam is a benzodiazepine used to treat anxiety and insomnia.",
          PregnancyCategory = PregnancyCategory.D,
          Stock= 20,
          Price= 4.61,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974972/da-net7/ozu64zuneatthlwqvmlr.png",
          ImageId= "da-net7/ozu64zuneatthlwqvmlr",
          ManufacturerId= 14,
          CategoryId = 3,
        },
        new Drug{
                Name= "Levothyroxine",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "50mcg",
          SideEffects = "weight loss, insomnia, headache",
          Contraindications= "thyrotoxicosis, acute myocardial infarction, uncorrected adrenal insufficiency",
          Description= "Levothyroxine is a thyroid hormone used to treat hypothyroidism and goiter.",
          PregnancyCategory = PregnancyCategory.A,
          Stock= 20,
          Price= 80.47,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680974997/da-net7/ocso29uk0ulm7yhxrl7p.png",
          ImageId= "da-net7/ocso29uk0ulm7yhxrl7p",
          ManufacturerId= 14,
          CategoryId = 7,
        },
        new Drug{
                Name= "Ibuprofen",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "200mg",
          SideEffects = "stomach upset, headache, dizziness",
          Contraindications= "asthma, kidney disease, heart disease",
          Description= "Ibuprofen is a nonsteroidal anti-inflammatory drug (NSAID) used to treat pain and inflammation.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 28.98,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680975006/da-net7/jwdid7fw8ckntruquwvr.png",
          ImageId= "da-net7/jwdid7fw8ckntruquwvr",
          ManufacturerId= 14,
          CategoryId = 1,
        },
        new Drug{
                Name= "Propranolol",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "40mg",
          SideEffects = "dizziness, fatigue, nausea",
          Contraindications= "asthma, heart block, liver disease",
          Description= "Propranolol is a beta blocker used to treat high blood pressure, heart rhythm disorders, and anxiety.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 53.16,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680975016/da-net7/h5q0darrvpxnzir28avu.png",
          ImageId= "da-net7/h5q0darrvpxnzir28avu",
          ManufacturerId= 15,
          CategoryId = 2,
        },
        new Drug{
                Name= "Azithromycin",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "250mg",
          SideEffects = "nausea, diarrhea, stomach pain",
          Contraindications= "liver disease, heart rhythm disorder, myasthenia gravis",
          Description= "Azithromycin is a macrolide antibiotic used to treat bacterial infections.",
          PregnancyCategory = PregnancyCategory.B,
          Stock= 20,
          Price= 63.5,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1680975023/da-net7/m66xq7lxay1lajua6d5m.png",
          ImageId= "da-net7/m66xq7lxay1lajua6d5m",
          ManufacturerId= 14,
          CategoryId = 4,
        },
        new Drug{
                Name= "RespiClear ",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "RespiClear is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 94.38,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658470/da-net7/lfunu1q4lfkcvbjtl56z.png",
          ImageId= "da-net7/lfunu1q4lfkcvbjtl56z",
          ManufacturerId= 3,
          CategoryId = 8,
        },
        new Drug{
                Name= "BreathEase ",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "BreathEase is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 74.54,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658492/da-net7/f437g2oqdpwgmqq4ivu2.png",
          ImageId= "da-net7/f437g2oqdpwgmqq4ivu2",
          ManufacturerId= 5,
          CategoryId = 8,
        },
        new Drug{
                Name= "DermaGlow",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "DermaGlow is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 55.04,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658508/da-net7/ofyjsnouiysei3279urt.png",
          ImageId= "da-net7/ofyjsnouiysei3279urt",
          ManufacturerId= 4,
          CategoryId = 9,
        },
        new Drug{
                Name= "SkinSoothe ",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "SkinSoothe is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 86.93,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658521/da-net7/butgcp3qoxwv26csqi4d.png",
          ImageId= "da-net7/butgcp3qoxwv26csqi4d",
          ManufacturerId= 4,
          CategoryId = 9,
        },
        new Drug{
                Name= "FemEase",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "FemEase is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 57.77,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658535/da-net7/b6u4dxzhe5lc3cy4lpoh.png",
          ImageId= "da-net7/b6u4dxzhe5lc3cy4lpoh",
          ManufacturerId= 1,
          CategoryId = 10,
        },
        new Drug{
                Name= "BellaFemme",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "BellaFemme is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 14.77,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658545/da-net7/fx0zxyfxlcnhl7v3qvq7.png",
          ImageId= "da-net7/fx0zxyfxlcnhl7v3qvq7",
          ManufacturerId= 7,
          CategoryId = 10,
        },
        new Drug{
                Name= "OncoGuard",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "OncoGuard is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 94.64,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658561/da-net7/zecfhuvlfbhe1ywtqyxg.png",
          ImageId= "da-net7/zecfhuvlfbhe1ywtqyxg",
          ManufacturerId= 2,
          CategoryId = 12,
        },
        new Drug{
                Name= "TumorStop",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "TumorStop is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 76.35,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658573/da-net7/sllvttc3dyzozo9n2yr2.png",
          ImageId= "da-net7/sllvttc3dyzozo9n2yr2",
          ManufacturerId= 20,
          CategoryId = 12,
        },
        new Drug{
                Name= "Pediatrex",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "Pediatrex is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 22.87,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658585/da-net7/fsv0lfx1mql7cba61sdn.png",
          ImageId= "da-net7/fsv0lfx1mql7cba61sdn",
          ManufacturerId= 14,
          CategoryId = 13,
        },
        new Drug{
                Name= "Kidz Care",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "Kidz Care is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 29.85,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658597/da-net7/byfg2vgyqzw0hzyqodif.png",
          ImageId= "da-net7/byfg2vgyqzw0hzyqodif",
          ManufacturerId= 10,
          CategoryId = 13,
        },
        new Drug{
                Name= "ImmunoShield",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "ImmunoShield is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 98.07,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658607/da-net7/kmv15dtkh0j4dwziof5v.png",
          ImageId= "da-net7/kmv15dtkh0j4dwziof5v",
          ManufacturerId= 17,
          CategoryId = 14,
        },
        new Drug{
                Name= "Immunotrex",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "Immunotrex is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 64.4,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658616/da-net7/w78mgsbplmfv0av7eltu.png",
          ImageId= "da-net7/w78mgsbplmfv0av7eltu",
          ManufacturerId= 6,
          CategoryId = 14,
        },
        new Drug{
                Name= "FlexiRelief",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "FlexiRelief is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 62.19,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658632/da-net7/r8jqygc1vpcbuucpdk5b.png",
          ImageId= "da-net7/r8jqygc1vpcbuucpdk5b",
          ManufacturerId= 11,
          CategoryId = 15,
        },
        new Drug{
                Name= "BoneEase",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "BoneEase is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 51.5,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658643/da-net7/trqbsv50q0sc28pgqos5.png",
          ImageId= "da-net7/trqbsv50q0sc28pgqos5",
          ManufacturerId= 19,
          CategoryId = 15,
        },
        new Drug{
                Name= "SoberGuard ",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "SoberGuard is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 78.17,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658653/da-net7/golmb1qvqd8kl4euq6kx.png",
          ImageId= "da-net7/golmb1qvqd8kl4euq6kx",
          ManufacturerId= 12,
          CategoryId = 16,
        },
        new Drug{
                Name= "AddictStop ",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "AddictStop is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 15.72,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658663/da-net7/ui9dgy6zutxq4ucjndkv.png",
          ImageId= "da-net7/ui9dgy6zutxq4ucjndkv",
          ManufacturerId= 13,
          CategoryId = 16,
        },
        new Drug{
                Name= "Eye Clear",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "Eye Clear is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 23.78,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658674/da-net7/ileyqktk12gst89xtwby.png",
          ImageId= "da-net7/ileyqktk12gst89xtwby",
          ManufacturerId= 8,
          CategoryId = 17,
        },
        new Drug{
                Name= "Visio Guard ",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "Visio Guard is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 21.1,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658687/da-net7/ywl0ejwwq94tqvgpytrv.png",
          ImageId= "da-net7/ywl0ejwwq94tqvgpytrv",
          ManufacturerId= 21,
          CategoryId = 17,
        },
        new Drug{
                Name= "Neuro Shield",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "Neuro Shield is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 11.8,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658695/da-net7/tmbbzvhdocjmydncztmh.png",
          ImageId= "da-net7/tmbbzvhdocjmydncztmh",
          ManufacturerId= 5,
          CategoryId = 18,
        },
        new Drug{
                Name= "Brain Ease",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "Brain Ease is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 45.66,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658705/da-net7/yosxvytsk1mcaghwinpm.png",
          ImageId= "da-net7/yosxvytsk1mcaghwinpm",
          ManufacturerId= 12,
          CategoryId = 18,
        },
        new Drug{
                Name= "Gastro Guard",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "Gastro Guard is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 95.52,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658719/da-net7/b6nvjg7pt7viezeismpr.png",
          ImageId= "da-net7/b6nvjg7pt7viezeismpr",
          ManufacturerId= 10,
          CategoryId = 19,
        },
        new Drug{
                Name= "DigestiEase",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "DigestiEase is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 70.55,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658730/da-net7/mont2zr8jgcios7x0qzp.png",
          ImageId= "da-net7/mont2zr8jgcios7x0qzp",
          ManufacturerId= 2,
          CategoryId = 19,
        },
        new Drug{
                Name= "ReproCare",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "ReproCare is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 11.94,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658742/da-net7/vz0kl1kunv1dw123n2x0.png",
          ImageId= "da-net7/vz0kl1kunv1dw123n2x0",
          ManufacturerId= 7,
          CategoryId = 20,
        },
        new Drug{
                Name= "FertiBoost",
          DosageForm = DosageForm.Tablet,
          DosageStrength= "500mg",
          SideEffects = "dizziness, headache, cough",
          Contraindications= "kidney disease, liver disease, alcoholism",
          Description= "FertiBoost is a substance, natural or synthetic, that is used to treat, prevent, or diagnose a medical condition or disease in humans or animals, prescribed by a licensed healthcare provider and dispensed by a licensed pharmacist.",
          PregnancyCategory = PregnancyCategory.C,
          Stock= 20,
          Price= 76.02,
          ImageURL= "https://res.cloudinary.com/dx3xe3xxp/image/upload/v1681658753/da-net7/kg9dx17gd0j5uvdylhom.png",
          ImageId= "da-net7/kg9dx17gd0j5uvdylhom",
          ManufacturerId= 3,
          CategoryId = 20,
        }
                };
                await _unitOfWork.Drug.AddRangeAsync(drugs);
                await _unitOfWork.SaveAsync();
                return Ok(new { success = true, message = "Drug Created Successfully", Drug = drugs });
            }*/
    }
}