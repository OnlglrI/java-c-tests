package com.example.shop.service;

import com.example.shop.dto.ProductDto;
import com.example.shop.model.Product;
import com.example.shop.repository.ProductRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class ProductService {

    private final ProductRepository productRepository;

    public List<ProductDto> getAll() {
        return productRepository.findAll().stream()
                .map(this::toDto)
                .collect(Collectors.toList());
    }

    public List<ProductDto> getFiltered(String search, BigDecimal minPrice, BigDecimal maxPrice,
                                         Boolean inStock, int skip, int limit) {
        List<Product> all = productRepository.findAll();

        List<Product> filtered = all.stream()
                .filter(p -> {
                    if (search != null && !search.isBlank()) {
                        return p.getName().toLowerCase().contains(search.toLowerCase());
                    }
                    return true;
                })
                .filter(p -> {
                    if (minPrice != null) {
                        return p.getPrice().compareTo(minPrice) >= 0;
                    }
                    return true;
                })
                .filter(p -> {
                    if (maxPrice != null) {
                        return p.getPrice().compareTo(maxPrice) <= 0;
                    }
                    return true;
                })
                .filter(p -> {
                    if (inStock != null && inStock) {
                        return p.getStock() > 0;
                    }
                    return true;
                })
                .collect(Collectors.toList());

        int fromIndex = Math.min(skip, filtered.size());
        int toIndex = Math.min(skip + limit, filtered.size());
        return filtered.subList(fromIndex, toIndex).stream()
                .map(this::toDto)
                .collect(Collectors.toList());
    }

    public ProductDto getById(Long id) {
        Product product = productRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Product not found"));
        return toDto(product);
    }

    public ProductDto create(ProductDto dto) {
        Product product = new Product();
        product.setName(dto.getName());
        product.setDescription(dto.getDescription());
        product.setPrice(dto.getPrice());
        product.setStock(dto.getStock());
        product.setCreatedAt(LocalDateTime.now());
        Product saved = productRepository.save(product);
        return toDto(saved);
    }

    public ProductDto update(Long id, ProductDto dto) {
        Product product = productRepository.findById(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Product not found"));
        product.setName(dto.getName());
        product.setDescription(dto.getDescription());
        product.setPrice(dto.getPrice());
        product.setStock(dto.getStock());
        Product saved = productRepository.save(product);
        return toDto(saved);
    }

    public void delete(Long id) {
        if (!productRepository.existsById(id)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Product not found");
        }
        productRepository.deleteById(id);
    }

    public ProductDto toDto(Product product) {
        ProductDto dto = new ProductDto();
        dto.setId(product.getId());
        dto.setName(product.getName());
        dto.setDescription(product.getDescription());
        dto.setPrice(product.getPrice());
        dto.setStock(product.getStock());
        return dto;
    }
}
