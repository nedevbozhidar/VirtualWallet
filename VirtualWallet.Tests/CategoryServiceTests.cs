using VirtualWallet.Business.DTOs.Categories;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Implementations;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Tests;

public class CategoryServiceTests
{
    [Fact]
    public async Task GetUserCategoriesAsync_ReturnsCategoriesForUser()
    {
        int userId = 1;
        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", UserId =  userId },
            new Category { Id = 2, Name = "Travel", UserId =  userId },
        };

        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        CategoryService service = new CategoryService(categoryRepository, null!);
        
        var result = await service.GetUserCategoriesAsync(userId);
        
        Assert.Equal(2, result.Count());
        Assert.Equal("Food", result.First().Name);
        Assert.Equal("Travel", result.Last().Name);
        
    }
    
    [Fact]
    public async Task GetUserCategoriesAsync_ReturnsEmptyCategoriesForUser()
    {
        int userId = 1;
        List<Category> categories = new List<Category>();
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        CategoryService service = new CategoryService(categoryRepository, null!);
        
        var result = await service.GetUserCategoriesAsync(userId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsCategory_WhenUserOwnsIt()
    {
        int userId = 1;
        int categoryId = 1;
        
        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", UserId = userId },
            new Category { Id = 2, Name = "Travel", UserId = userId }
        };
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        CategoryService service = new CategoryService(categoryRepository, null!);

        var result = await service.GetCategoryByIdAsync(userId, categoryId);
        
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("Food", result.Name);
    }
    
    [Fact]
    public async Task GetCategoryByIdAsync_ThrowsEntityNotFoundException_WhenCategoryDoesNotExist()
    {
        int userId = 1;
        int categoryId = 3;
        
        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", UserId = userId },
            new Category { Id = 2, Name = "Travel", UserId = userId },
        };
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        CategoryService service = new CategoryService(categoryRepository, null!);
        
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => service.GetCategoryByIdAsync(userId, categoryId));
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ThrowsUnauthorizedOperationException_WhenUserDoesNotOwnCategory()
    {
        int userId = 1;
        int categoryId = 1;

        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", UserId = 2 },
            new Category { Id = 2, Name = "Travel", UserId = 2 },
        };
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        CategoryService service = new CategoryService(categoryRepository, null!);
        
        await Assert.ThrowsAsync<UnauthorizedOperationException>(
            () => service.GetCategoryByIdAsync(userId, categoryId));
    }

    [Fact]
    public async Task CreateCategoryAsync_ReturnsCategory_WhenCategoryIsCreated()
    {
        int userId = 1;
        
        List<User> users = new List<User>
        {
            new User { Id = userId }
        };

        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        FakeUserRepository userRepository = new FakeUserRepository(users);
        CategoryService service = new CategoryService(categoryRepository, userRepository);

        CategoryCreateDto categoryCreateDto = new CategoryCreateDto { Name = "Food" };
        
        var result = await service.CreateCategoryAsync(userId, categoryCreateDto);
        var savedCategories = await categoryRepository.GetByUserIdAsync(userId);
        
        Assert.Equal("Food", result.Name);
        Assert.Single(savedCategories);        
    }

    [Fact]
    public async Task CreateCategoryAsync_ThrowsDuplicateEntityException_WhenCategoryAlreadyExists()
    {
        int userId = 1;

        List<User> users = new List<User>
        {
            new User { Id = userId }
        };

        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", UserId = userId },
        };
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        CategoryService service = new CategoryService(categoryRepository, userRepository);
        
        CategoryCreateDto categoryCreateDto = new CategoryCreateDto { Name = "food" };
        
        await  Assert.ThrowsAsync<DuplicateEntityException>(
            () => service.CreateCategoryAsync(userId, categoryCreateDto));
    }

    [Fact]
    public async Task CreateCategoryAsync_ThrowsEntityNotFoundException_WhenUserDoesNotExist()
    {
        int userId = 1;
        int missingUserId = 2;

        List<User> users = new List<User>
        {
            new User { Id = userId }
        };
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(new List<Category>());
        FakeUserRepository userRepository = new FakeUserRepository(users);
        CategoryService service = new CategoryService(categoryRepository, userRepository);
        
        CategoryCreateDto categoryCreateDto = new CategoryCreateDto { Name = "Travel" };
        
        await  Assert.ThrowsAsync<EntityNotFoundException>(
            () => service.CreateCategoryAsync(missingUserId, categoryCreateDto));
    }

    [Fact]
    public async Task UpdateCategoryAsync_ReturnsCategory_WhenCategoryIsUpdated()
    {
        int userId = 1;
        int categoryId = 1;

        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", UserId = userId }
        };

        List<User> users = new List<User>
        {
            new User { Id = userId }
        };
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        CategoryService service = new CategoryService(categoryRepository, userRepository);
        
        CategoryUpdateDto categoryUpdateDto = new CategoryUpdateDto { Name = "Travel" };
        
        var result = await service.UpdateCategoryAsync(userId, categoryId, categoryUpdateDto);
        var savedCategories = await categoryRepository.GetByIdAsync(categoryId);
        
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("Travel", result.Name);
        Assert.Equal("Travel", savedCategories!.Name);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ThrowsDuplicateEntityException_WhenCategoryAlreadyExists()
    {
        int userId = 1;
        int categoryId = 1;
        List<User> users = new List<User>
        {
            new User { Id = userId }
        };

        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", UserId = userId },
            new  Category { Id = 2, Name = "Travel", UserId = userId }
        };
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        CategoryService service = new CategoryService(categoryRepository, userRepository);
        
        CategoryUpdateDto categoryUpdateDto = new CategoryUpdateDto() { Name = "travel" };
        
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => service.UpdateCategoryAsync(userId, categoryId, categoryUpdateDto));
    }
    
    [Fact]
    public async Task UpdateCategoryAsync_ThrowsEntityNotFoundException_WhenCategoryDoesNotExist()
    {
        int userId = 1;
        int missingCategoryId = 2;

        List<User> users = new List<User>
        {
            new User { Id = userId }
        };
        
        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", UserId = userId },
        };
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        CategoryService service = new CategoryService(categoryRepository, userRepository);
        
        CategoryUpdateDto categoryUpdateDto = new CategoryUpdateDto() { Name = "Travel" };
        
        await  Assert.ThrowsAsync<EntityNotFoundException>(
            () => service.UpdateCategoryAsync(userId, missingCategoryId, categoryUpdateDto));
    }
    
    [Fact]
    public async Task UpdateCategoryAsync_ThrowsUnauthorizedOperationException_WhenUserDoesNotOwnCategory()
    {
        int userId = 1;
        int unauthiorizedUserId = 2;
        int categoryId = 1;

        List<User> users = new List<User>
        {
            new User { Id = userId },
            new User { Id = unauthiorizedUserId }
        };
        
        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", UserId = userId },
            new  Category { Id = 2, Name = "Travel", UserId = unauthiorizedUserId }
        };
        
        FakeCategoryRepository categoryRepository = new FakeCategoryRepository(categories);
        FakeUserRepository userRepository = new FakeUserRepository(users);
        CategoryService service = new CategoryService(categoryRepository, userRepository);
        
        CategoryUpdateDto categoryUpdateDto = new CategoryUpdateDto() { Name = "Travel" };
        
        await  Assert.ThrowsAsync<UnauthorizedOperationException>(
            () => service.UpdateCategoryAsync(unauthiorizedUserId, categoryId, categoryUpdateDto));
    }
}

public class FakeCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categories;
    
    public FakeCategoryRepository(List<Category> categories)
    {
        _categories = categories;
    }
    
    public async Task<Category?> GetByIdAsync(int id)
    {
        return await Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
    }

    public async Task<IEnumerable<Category>> GetByUserIdAsync(int userId)
    {
        return await Task.FromResult(_categories.Where(c => c.UserId == userId));
    }

    public Task AddAsync(Category category)
    {
        _categories.Add(category);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Category category)
    {
        int index = _categories.FindIndex(c => c.Id == category.Id);

        if (index < 0)
        {
            throw new InvalidOperationException(
                $"Category with id {category.Id} was not found.");
        }
        
        _categories[index] = category;
        return Task.CompletedTask;
    }
}

public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users;
    
    public FakeUserRepository(List<User> users)
    {
        _users = users;
    }
    
    public Task<IEnumerable<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    }

    public Task<User?> GetByIdWithDetailsAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}