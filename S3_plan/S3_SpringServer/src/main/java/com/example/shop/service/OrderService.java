package com.example.shop.service;

import com.example.shop.dto.CreateOrderRequest;
import com.example.shop.dto.OrderDto;
import com.example.shop.model.Order;
import com.example.shop.model.Product;
import com.example.shop.model.User;
import com.example.shop.repository.OrderRepository;
import com.example.shop.repository.ProductRepository;
import com.example.shop.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.server.ResponseStatusException;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class OrderService {

    private final OrderRepository orderRepository;
    private final UserRepository userRepository;
    private final ProductRepository productRepository;

    public List<OrderDto> getOrdersForUser(String username) {
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found"));
        return orderRepository.findByUserId(user.getId()).stream()
                .map(this::toDto)
                .collect(Collectors.toList());
    }

    @Transactional
    public OrderDto createOrder(String username, CreateOrderRequest request) {
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "User not found"));

        Product product = productRepository.findById(request.getProductId())
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Product not found"));

        if (product.getStock() < request.getQuantity()) {
            throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Not enough stock");
        }

        product.setStock(product.getStock() - request.getQuantity());
        productRepository.save(product);

        BigDecimal totalPrice = product.getPrice().multiply(BigDecimal.valueOf(request.getQuantity()));

        Order order = new Order();
        order.setUser(user);
        order.setProduct(product);
        order.setQuantity(request.getQuantity());
        order.setTotalPrice(totalPrice);
        order.setCreatedAt(LocalDateTime.now());

        Order saved = orderRepository.save(order);
        return toDto(saved);
    }

    public void deleteOrder(Long id) {
        if (!orderRepository.existsById(id)) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND, "Order not found");
        }
        orderRepository.deleteById(id);
    }

    private OrderDto toDto(Order order) {
        OrderDto dto = new OrderDto();
        dto.setId(order.getId());
        dto.setProductId(order.getProduct().getId());
        dto.setProductName(order.getProduct().getName());
        dto.setQuantity(order.getQuantity());
        dto.setTotalPrice(order.getTotalPrice());
        dto.setStatus(order.getStatus().name());
        return dto;
    }
}
