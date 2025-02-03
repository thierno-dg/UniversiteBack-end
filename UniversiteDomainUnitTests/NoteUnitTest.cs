using Moq;
using NUnit.Framework;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UniversiteDomainUnitTests
{
    public class NoteUnitTest
    {
        private Mock<IRepositoryFactory> mockFactory;
        private Mock<IEtudiantRepository> mockEtudiantRepo;
        private Mock<IUeRepository> mockUeRepo;
        private Mock<INoteRepository> mockNoteRepo;

        [SetUp]
        public void SetUp()
        {
            mockFactory = new Mock<IRepositoryFactory>();
            mockEtudiantRepo = new Mock<IEtudiantRepository>();
            mockUeRepo = new Mock<IUeRepository>();
            mockNoteRepo = new Mock<INoteRepository>();
        }
        
        [Test]
        public void CreateNote_InvalidNoteValue()
        {
            // Arrange
            var useCase = new CreateNoteUseCase(mockFactory.Object);

            // Act & Assert
            Assert.ThrowsAsync<InvalidNoteException>(async () => await useCase.ExecuteAsync(1, 1, -1));
        }

        [Test]
        public async Task CreateNote_EtudiantNotFound()
        {
            // Arrange
            long etudiantId = 1;
            long ueId = 1;
            float noteValeur = 15;
            mockEtudiantRepo.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync((Etudiant)null);
            mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);

            var useCase = new CreateNoteUseCase(mockFactory.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<EtudiantNotFoundException>(async () => await useCase.ExecuteAsync(etudiantId, ueId, noteValeur));
            Assert.AreEqual($"Étudiant avec l'ID {etudiantId} introuvable.", ex.Message);
        }

        [Test]
        public async Task CreateNote_UeNotFound()
        {
            // Arrange
            long etudiantId = 1;
            long ueId = 1;
            float noteValeur = 15;
            var etudiant = new Etudiant { Id = etudiantId };
            mockEtudiantRepo.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(etudiant);
            mockUeRepo.Setup(repo => repo.FindAsync(ueId)).ReturnsAsync((Ue)null);
            mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
            mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);

            var useCase = new CreateNoteUseCase(mockFactory.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UeNotFoundException>(async () => await useCase.ExecuteAsync(etudiantId, ueId, noteValeur));
            Assert.AreEqual($"UE avec l'ID {ueId} introuvable.", ex.Message);
        }

        [Test]
        public async Task CreateNote_ExistingNoteForStudentInUe()
        {
            // Arrange
            long etudiantId = 1;
            long ueId = 1;
            float noteValeur = 15;
            var etudiant = new Etudiant { Id = etudiantId };
            var ue = new Ue { Id = ueId };
            mockEtudiantRepo.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(etudiant);
            mockUeRepo.Setup(repo => repo.FindAsync(ueId)).ReturnsAsync(ue);

            // Corrigez cette partie : utilisez Expression<Func<Note, bool>>
            mockNoteRepo.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
                .ReturnsAsync(new List<Note> { new Note {  Valeur = 15, EtudiantId = etudiantId, UeId = ueId } });

            mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
            mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
            mockFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);

            var useCase = new CreateNoteUseCase(mockFactory.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<DuplicateNotePourUeException>(async () => await useCase.ExecuteAsync(etudiantId, ueId, noteValeur));
            Assert.AreEqual($"Une note existe déjà pour l'étudiant (ID: {etudiantId}) dans l'UE (ID: {ueId}).", ex.Message);
        }


       
    }
}
