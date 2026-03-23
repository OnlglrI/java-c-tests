package com.example.shop.dto;

import lombok.Data;
import java.math.BigDecimal;

@Data
public class OrderDto {
    private Long id;
    private Long userId;
    private Long productId;
    private String productName;
    private int quantity;
    private BigDecimal totalPrice;
    private String status;
}
